using HomeNet.Core.Common;
using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Auth.Commands;
using HomeNet.Core.Modules.Auth.Models;
using HomeNet.Core.Modules.Persons.Abstractions;
using HomeNet.Core.Modules.Persons.Models;
using Moq;

namespace HomeNet.Core.Test.Modules.Auth.Commands;

public class LinkPersonToUserTest
{
    private static readonly Person person = new()
    {
        Id = 1,
        FirstName = "John",
        LastName = "Doe",
        AliasName = "JD",
        IsInactive = false,
    };
    
    private static readonly User user = new()
    {
        Id = 1,
        UserName = "testuser",
        PasswordHash = "hashedpassword",
        Role = "User",
        PersonId = person.Id,
    };

    private LinkPersonToUser.CommandHandler _handler;

    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IPersonRepository> _personRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _personRepositoryMock = new Mock<IPersonRepository>();

        _handler = new LinkPersonToUser.CommandHandler(
            _userRepositoryMock.Object, 
            _personRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var ct = new CancellationToken();

        var command = new LinkPersonToUser.Command
        {
            UserName = user.UserName,
            PersonId = user.PersonId!.Value,
        };

        _personRepositoryMock
            .Setup(x => x.GetPersonByIdAsync(command.PersonId, ct))
            .ReturnsAsync(person);

        _userRepositoryMock
            .Setup(x => x.GetUserByUsernameAsync(command.UserName, ct))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.UpdatePersonLinkAsync(user.Id, person.Id, ct))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.HandleAsync(command, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);
        });

        _personRepositoryMock.Verify(
            x => x.GetPersonByIdAsync(command.PersonId, ct), 
            Times.Once);

        _userRepositoryMock.Verify(
            x => x.GetUserByUsernameAsync(command.UserName, ct), 
            Times.Once);
        
        _userRepositoryMock.Verify(
            x => x.UpdatePersonLinkAsync(user.Id, person.Id, ct), 
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_UserNotFoundAsync()
    {
        // Arrange
        var ct = new CancellationToken();

        var command = new LinkPersonToUser.Command
        {
            UserName = user.UserName,
            PersonId = user.PersonId!.Value,
        };

        _userRepositoryMock
            .Setup(x => x.GetUserByUsernameAsync(command.UserName, ct))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.HandleAsync(command, ct);

        // Assert
        _personRepositoryMock.Verify(
            x => x.GetPersonByIdAsync(command.PersonId, ct), 
            Times.Never);

        _userRepositoryMock.Verify(
            x => x.GetUserByUsernameAsync(command.UserName, ct), 
            Times.Once);
        
        _userRepositoryMock.Verify(
            x => x.UpdatePersonLinkAsync(user.Id, person.Id, ct), 
            Times.Never);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_PersonNotFoundAsync()
    {
        // Arrange
        var ct = new CancellationToken();

        var command = new LinkPersonToUser.Command
        {
            UserName = user.UserName,
            PersonId = user.PersonId!.Value,
        };

        _personRepositoryMock
            .Setup(x => x.GetPersonByIdAsync(command.PersonId, ct))
            .ReturnsAsync((Person?)null);

        _userRepositoryMock
            .Setup(x => x.GetUserByUsernameAsync(command.UserName, ct))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.HandleAsync(command, ct);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.Not.Null);
        });

        _personRepositoryMock.Verify(
            x => x.GetPersonByIdAsync(command.PersonId, ct), 
            Times.Once);

        _userRepositoryMock.Verify(
            x => x.GetUserByUsernameAsync(command.UserName, ct), 
            Times.Once);
        
        _userRepositoryMock.Verify(
            x => x.UpdatePersonLinkAsync(user.Id, person.Id, ct), 
            Times.Never);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_InvalidCommand()
    {
        // Arrange
        var commandInvalidUsername = new LinkPersonToUser.Command
        {
            UserName = string.Empty,
            PersonId = 1,
        };
        var commandInvalidPersonId = new LinkPersonToUser.Command
        {
            UserName = "testuser",
            PersonId = 0,
        };

        // Act
        var resultInvalidUserName = await _handler.HandleAsync(commandInvalidUsername);
        var resultInvalidPersonId = await _handler.HandleAsync(commandInvalidPersonId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultInvalidUserName.IsSuccess, Is.False);
            Assert.That(resultInvalidUserName.Error, Is.Not.Null);

            Assert.That(resultInvalidPersonId.IsSuccess, Is.False);
            Assert.That(resultInvalidPersonId.Error, Is.Not.Null);
        });

        _userRepositoryMock.Verify(
            x => x.GetUserByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), 
            Times.Never);
        
        _userRepositoryMock.Verify(
            x => x.UpdatePersonLinkAsync(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }
}