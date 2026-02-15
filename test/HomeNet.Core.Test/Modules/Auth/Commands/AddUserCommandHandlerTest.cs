using HomeNet.Core.Common;
using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Auth.Commands;
using HomeNet.Core.Modules.Auth.Models;
using Moq;

namespace HomeNet.Core.Test.Modules.Auth.Commands;

public class AddUserCommandHandlerTest
{
    private AddUserCommandHandler _handler;

    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IPasswordService> _passwordServiceMock;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordServiceMock = new Mock<IPasswordService>();

        _handler = new AddUserCommandHandler(
            _userRepositoryMock.Object, 
            _passwordServiceMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var command1 = new AddUserCommand
        {
            UserName = "testuser",
            Password = "hashedpassword",
            Role = "User",
        };
        var command2 = new AddUserCommand
        {
            UserName = "otherUser",
            Password = "otherHashedpassword",
            Role = "Admin",
        };

        _userRepositoryMock
            .Setup(x => x.AddUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var resultCommand1 = await _handler.HandleAsync(command1);
        var resultCommand2 = await _handler.HandleAsync(command2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultCommand1.IsSuccess, Is.True);
            Assert.That(resultCommand1.Error, Is.Null);

            Assert.That(resultCommand2.IsSuccess, Is.True);
            Assert.That(resultCommand2.Error, Is.Null);
        });

        _userRepositoryMock.Verify(
            x => x.AddUserAsync(
                It.Is<User>(u =>
                    u.UserName == command1.UserName &&
                    u.PasswordHash == command1.Password &&
                    u.Role == command1.Role),
                It.IsAny<CancellationToken>()),
            Times.Once);
        
        _userRepositoryMock.Verify(
            x => x.AddUserAsync(
                It.Is<User>(u =>
                    u.UserName == command2.UserName &&
                    u.PasswordHash == command2.Password &&
                    u.Role == command2.Role),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_InvalidCommand()
    {
        // Arrange
        var commandInvalidUserName = new AddUserCommand
        {
            UserName = string.Empty,
            Password = "hashedpassword",
            Role = "User",
        };
        var commandInvalidPassword = new AddUserCommand
        {
            UserName = "testuser",
            Password = string.Empty,
            Role = "User",
        };
        var commandInvalidRole = new AddUserCommand
        {
            UserName = "testuser",
            Password = "hashedpassword",
            Role = "SuperUser",
        };

        _userRepositoryMock
            .Setup(x => x.AddUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

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
