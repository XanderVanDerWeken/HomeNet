using HomeNet.Core.Common;
using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Auth.Commands;
using HomeNet.Core.Modules.Auth.Models;
using Moq;

namespace HomeNet.Core.Test.Modules.Auth.Commands;

public class UnlinkPersonFromUserCommandHandlerTest
{
    private static readonly User user = new()
    {
        Id = 1,
        UserName = "testuser",
        PasswordHash = "hashedpassword",
        Role = "User",
        PersonId = 1,
    };

    private UnlinkPersonFromUserCommandHandler _handler;

    private Mock<IUserRepository> _userRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();

        _handler = new UnlinkPersonFromUserCommandHandler(
            _userRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var ct = new CancellationToken();
        
        var command = new UnlinkPersonFromUserCommand
        {
            UserName = user.UserName,
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByUsernameAsync(command.UserName, ct))
            .ReturnsAsync(user);
        
        _userRepositoryMock
            .Setup(x => x.UpdatePersonLinkAsync(user.Id, null, ct))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.HandleAsync(command, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);
        });

        _userRepositoryMock.Verify(
            x => x.GetUserByUsernameAsync(command.UserName, ct), 
            Times.Once);
        
        _userRepositoryMock.Verify(
            x => x.UpdatePersonLinkAsync(user.Id, null, ct), 
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_UserNotFound()
    {
        // Arrange
        var ct = new CancellationToken();
        
        var command = new UnlinkPersonFromUserCommand
        {
            UserName = user.UserName,
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByUsernameAsync(command.UserName, ct))
            .ReturnsAsync((User?)null);
        
        // Act
        var result = await _handler.HandleAsync(command, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.Not.Null);
        });

        _userRepositoryMock.Verify(
            x => x.GetUserByUsernameAsync(command.UserName, ct), 
            Times.Once);
        
        _userRepositoryMock.Verify(
            x => x.UpdatePersonLinkAsync(user.Id, null, ct), 
            Times.Never);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_InvalidCommand()
    {
        // Arrange
        var commandInvalidUsername = new UnlinkPersonFromUserCommand
        {
            UserName = string.Empty,
        };

        // Act
        var resultInvalidUserName = await _handler.HandleAsync(commandInvalidUsername);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultInvalidUserName.IsSuccess, Is.False);
            Assert.That(resultInvalidUserName.Error, Is.Not.Null);
        });

        _userRepositoryMock.Verify(
            x => x.GetUserByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), 
            Times.Never);
        
        _userRepositoryMock.Verify(
            x => x.UpdatePersonLinkAsync(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }
}
