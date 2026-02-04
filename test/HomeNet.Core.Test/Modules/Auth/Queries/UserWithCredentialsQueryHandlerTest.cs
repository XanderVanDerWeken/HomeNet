using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Auth.Models;
using HomeNet.Core.Modules.Auth.Queries;
using Moq;

namespace HomeNet.Core.Test.Modules.Auth.Queries;

public class UserWithCredentialsQueryHandlerTest
{
    private UserWithCredentialsQueryHandler _handler;

    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IPasswordService> _passwordServiceMock;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordServiceMock = new Mock<IPasswordService>();

        _handler = new UserWithCredentialsQueryHandler(
            _userRepositoryMock.Object, 
            _passwordServiceMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsBool_CredentialsMatch()
    {
        // Arrange
        var ct = new CancellationToken();

        var user = new User
        {
            Id = 1,
            UserName = "testuser",
            PasswordHash = "hashedpassword",
        };

        var query = new UserWithCredentialsQuery
        {
            UserName = "testuser",
            Password = "password123",
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByUsernameAsync(query.UserName, ct))
            .ReturnsAsync(user);
        
        _passwordServiceMock
            .Setup(x => x.VerifyPassword(query.Password, user.PasswordHash))
            .Returns(true);

        // Act
        var result = await _handler.HandleAsync(query, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);

            Assert.That(result.Value, Is.True);
        });

        _userRepositoryMock.Verify(
            x => x.GetUserByUsernameAsync(query.UserName, ct), 
            Times.Once);
        
        _passwordServiceMock.Verify(
            x => x.VerifyPassword(query.Password, user.PasswordHash),
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsBool_UserNotFound()
    {
        // Arrange
        var ct = new CancellationToken();

        var user = new User
        {
            Id = 1,
            UserName = "testuser",
            PasswordHash = "hashedpassword",
        };

        var query = new UserWithCredentialsQuery
        {
            UserName = "testuser",
            Password = "password123",
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByUsernameAsync(query.UserName, ct))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.HandleAsync(query, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);

            Assert.That(result.Value, Is.False);
        });

        _userRepositoryMock.Verify(
            x => x.GetUserByUsernameAsync(query.UserName, ct), 
            Times.Once);
        
        _passwordServiceMock.Verify(
            x => x.VerifyPassword(query.Password, user.PasswordHash),
            Times.Never);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsBool_PasswordMismatch()
    {
        // Arrange
        var ct = new CancellationToken();

        var user = new User
        {
            Id = 1,
            UserName = "testuser",
            PasswordHash = "hashedpassword",
        };

        var query = new UserWithCredentialsQuery
        {
            UserName = "testuser",
            Password = "notSamePassword1",
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByUsernameAsync(query.UserName, ct))
            .ReturnsAsync(user);
        
        _passwordServiceMock
            .Setup(x => x.VerifyPassword(query.Password, user.PasswordHash))
            .Returns(false);

        // Act
        var result = await _handler.HandleAsync(query, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);

            Assert.That(result.Value, Is.False);
        });

        _userRepositoryMock.Verify(
            x => x.GetUserByUsernameAsync(query.UserName, ct), 
            Times.Once);
        
        _passwordServiceMock.Verify(
            x => x.VerifyPassword(query.Password, user.PasswordHash),
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_InvalidQuery()
    {
        // Arrange
        var queryInvalidUserName = new UserWithCredentialsQuery
        {
            UserName = string.Empty,
            Password = "password123",
        };
        var queryInvalidPassword = new UserWithCredentialsQuery
        {
            UserName = "testuser",
            Password = string.Empty,
        };

        // Act
        var resultInvalidUserName = await _handler.HandleAsync(queryInvalidUserName);
        var resultInvalidPassword = await _handler.HandleAsync(queryInvalidPassword);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultInvalidUserName.IsSuccess, Is.False);
            Assert.That(resultInvalidUserName.Error, Is.Not.Null);

            Assert.That(resultInvalidPassword.IsSuccess, Is.False);
            Assert.That(resultInvalidPassword.Error, Is.Not.Null);
        });
    }
}
