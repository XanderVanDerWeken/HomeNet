using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Auth.Commands;
using HomeNet.Core.Modules.Auth.Models;
using Moq;

namespace HomeNet.Core.Test.Modules.Auth.Commands;

public class AddUserCommandHandlerTest
{
    private AddUserCommandHandler _handler;

    private Mock<IUserRepository> _userRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();

        _handler = new AddUserCommandHandler(_userRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var command = new AddUserCommand
        {
            Username = "testuser",
            PasswordHash = "HashedPassword",
            Role = UserRole.User,
        };

        _userRepositoryMock
            .Setup(r => r.AddUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Core.Common.Result.Success());

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);
        });

        _userRepositoryMock.Verify(
            r => r.AddUserAsync(
                It.Is<User>(u =>
                    u.Username == command.Username &&
                    u.PasswordHash == command.PasswordHash &&
                    u.Role == command.Role),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_InvalidCommand()
    {
        // Arrange
        var commandInvalidUsername = new AddUserCommand
        {
            Username = string.Empty,
            PasswordHash = "HashedPassword",
            Role = UserRole.User,
        };
        var commandInvalidPassword = new AddUserCommand
        {
            Username = "testuser",
            PasswordHash = string.Empty,
            Role = UserRole.User,
        };

        _userRepositoryMock
            .Setup(r => r.AddUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Core.Common.Result.Success());

        // Act
        var resultInvalidUsername = await _handler.HandleAsync(commandInvalidUsername);
        var resultInvalidPassword = await _handler.HandleAsync(commandInvalidPassword);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultInvalidUsername.IsSuccess, Is.False);
            Assert.That(resultInvalidUsername.Error, Is.Not.Null);

            Assert.That(resultInvalidPassword.IsSuccess, Is.False);
            Assert.That(resultInvalidPassword.Error, Is.Not.Null);
        });

        _userRepositoryMock.Verify(
            r => r.AddUserAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
