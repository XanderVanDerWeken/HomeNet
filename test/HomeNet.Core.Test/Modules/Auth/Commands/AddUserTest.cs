using HomeNet.Core.Common;
using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Auth.Commands;
using HomeNet.Core.Modules.Auth.Models;
using Moq;

namespace HomeNet.Core.Test.Modules.Auth.Commands;

public class AddUserTest
{
    private AddUser.CommandHandler _handler;

    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IPasswordService> _passwordServiceMock;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordServiceMock = new Mock<IPasswordService>();

        _handler = new AddUser.CommandHandler(
            _userRepositoryMock.Object, 
            _passwordServiceMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess_User()
    {
        // Arrange
        var command = new AddUser.Command
        {
            UserName = "testuser",
            Password = "password",
            Role = "User",
        };

        var hashedPassword = "hashedpassword1";

        _passwordServiceMock
            .Setup(x => x.HashPassword(command.Password))
            .Returns(hashedPassword);

        var user = new User
        {
            Id = 1,
            UserName = command.UserName,
            PasswordHash = hashedPassword,
            Role = command.Role
        };

        _userRepositoryMock
            .Setup(x => x.AddUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(user));

        // Act
        var resultCommand = await _handler.HandleAsync(command);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultCommand.IsSuccess, Is.True);
            Assert.That(resultCommand.Error, Is.Null);
        });

        _userRepositoryMock.Verify(
            x => x.AddUserAsync(
                It.Is<User>(u =>
                    u.UserName == command.UserName &&
                    u.PasswordHash == hashedPassword &&
                    u.Role == command.Role),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess_Admin()
    {
        // Arrange
        var command = new AddUser.Command
        {
            UserName = "otherUser",
            Password = "otherPassword",
            Role = "Admin",
        };

        var hashedPassword = "hashedpassword";

        _passwordServiceMock
            .Setup(x => x.HashPassword(command.Password))
            .Returns(hashedPassword);

        var user = new User
        {
            Id = 1,
            UserName = command.UserName,
            PasswordHash = hashedPassword,
            Role = command.Role
        };

        _userRepositoryMock
            .Setup(x => x.AddUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(user));

        // Act
        var resultCommand1 = await _handler.HandleAsync(command);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultCommand1.IsSuccess, Is.True);
            Assert.That(resultCommand1.Error, Is.Null);
        });

        _userRepositoryMock.Verify(
            x => x.AddUserAsync(
                It.Is<User>(u =>
                    u.UserName == command.UserName &&
                    u.PasswordHash == hashedPassword &&
                    u.Role == command.Role),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_InvalidCommand()
    {
        // Arrange
        var commandInvalidUserName = new AddUser.Command
        {
            UserName = string.Empty,
            Password = "hashedpassword",
            Role = "User",
        };
        var commandInvalidPassword = new AddUser.Command
        {
            UserName = "testuser",
            Password = string.Empty,
            Role = "User",
        };
        var commandInvalidRole = new AddUser.Command
        {
            UserName = "testuser",
            Password = "hashedpassword",
            Role = "SuperUser",
        };

        // Act
        var resultInvalidUserName = await _handler.HandleAsync(commandInvalidUserName);
        var resultInvalidPassword = await _handler.HandleAsync(commandInvalidPassword);
        var resultInvalidRole = await _handler.HandleAsync(commandInvalidRole);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultInvalidUserName.IsSuccess, Is.False);
            Assert.That(resultInvalidUserName.Error, Is.Not.Null);

            Assert.That(resultInvalidPassword.IsSuccess, Is.False);
            Assert.That(resultInvalidPassword.Error, Is.Not.Null);

            Assert.That(resultInvalidRole.IsSuccess, Is.False);
            Assert.That(resultInvalidRole.Error, Is.Not.Null);
        });

        _userRepositoryMock.Verify(
            x => x.AddUserAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
