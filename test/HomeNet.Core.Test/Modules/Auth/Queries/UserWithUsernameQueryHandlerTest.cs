using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Auth.Models;
using HomeNet.Core.Modules.Auth.Queries;
using Moq;

namespace HomeNet.Core.Test.Modules.Auth.Queries;

public class UserWithUsernameQueryHandlerTest
{
    private UserWithUsernameQueryHandler _handler;

    private Mock<IUserRepository> _userRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();

        _handler = new UserWithUsernameQueryHandler(_userRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var foundUsername = "testuser";
        var notFoundUsername = "nonexistentuser";
        
        var foundUser = new User
        {
            Id = 1,
            Username = foundUsername,
            PasswordHash = "HashedPassword",
            Role = UserRole.User,
        };

        var queryFoundUsername = new UserWithUsernameQuery
        {
            Username = foundUsername,
        };

        var queryNotFoundUsername = new UserWithUsernameQuery
        {
            Username = notFoundUsername,
        };

        _userRepositoryMock
            .Setup(r => r.GetUserByUsername(foundUsername, It.IsAny<CancellationToken>()))
            .ReturnsAsync(foundUser);
        
        _userRepositoryMock
            .Setup(r => r.GetUserByUsername(notFoundUsername, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var resultFoundUsername = await _handler.HandleAsync(queryFoundUsername);
        var resultNotFoundUsername = await _handler.HandleAsync(queryNotFoundUsername);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultFoundUsername.IsSuccess, Is.True);
            Assert.That(resultFoundUsername.Value, Is.Not.Null);
            Assert.That(resultFoundUsername.Value, Is.EqualTo(foundUser));

            Assert.That(resultNotFoundUsername.IsSuccess, Is.True);
            Assert.That(resultNotFoundUsername.Value, Is.Null);
        });

        _userRepositoryMock.Verify(
            r => r.GetUserByUsername(
                foundUsername,
                It.IsAny<CancellationToken>()),
            Times.Once);
        
        _userRepositoryMock.Verify(
            r => r.GetUserByUsername(
                notFoundUsername,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_InvalidQuery()
    {
        // Arrange
        var query = new UserWithUsernameQuery
        {
            Username = string.Empty,
        };

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.Not.Null);
        });

        _userRepositoryMock.Verify(
            r => r.GetUserByUsername(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
