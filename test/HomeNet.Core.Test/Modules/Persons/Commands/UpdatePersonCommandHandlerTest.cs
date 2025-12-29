using HomeNet.Core.Modules.Persons.Abstractions;
using HomeNet.Core.Modules.Persons.Commands;
using HomeNet.Core.Modules.Persons.Models;
using Moq;

namespace HomeNet.Core.Test.Modules.Persons.Commands;

public class UpdatePersonCommandHandlerTest
{
    private UpdatePersonCommandHandler _handler;

    private Mock<IPersonRepository> _personRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _personRepositoryMock = new Mock<IPersonRepository>();

        _handler = new UpdatePersonCommandHandler(_personRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var person = new Person
        {
            Id = 1,
            FirstName = "First",
            LastName = "Last",
            AliasName = null,
            IsInactive = false,
        };

        var command = new UpdatePersonCommand
        {
            PersonId = person.Id,
            UpdatedFirstName = "Jane",
            UpdatedLastName = "Smith",
            UpdatedAliasName = "JS",
            UpdatedIsInactive = true,
        };

        _personRepositoryMock
            .Setup(r => r.GetPersonByIdAsync(person.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        _personRepositoryMock
            .Setup(r => r.UpdatePersonAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Core.Common.Result.Success());

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);
        });

        _personRepositoryMock.Verify(
            r => r.GetPersonByIdAsync(person.Id, It.IsAny<CancellationToken>()),
            Times.Once);
        
        _personRepositoryMock.Verify(
            r => r.UpdatePersonAsync(person, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestCase(null, "Smith", "JS")]
    [TestCase("Jane", null, "JS")]
    [TestCase("Jane", "Smith", null)]
    public async Task Should_HandleAsync_ReturnsFailure_InvalidCommand(
        string? updatedFirstName,
        string? updatedLastName,
        string? updatedAliasName)
    {
        // Arrange
        var command = new UpdatePersonCommand
        {
            PersonId = 1,
            UpdatedFirstName = updatedFirstName,
            UpdatedLastName = updatedLastName,
            UpdatedAliasName = updatedAliasName,
            UpdatedIsInactive = true,
        };

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.Not.Null);
        });
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_PersonNotFound()
    {
        // Arrange
        var invalidId = 999;

        var command = new UpdatePersonCommand
        {
            PersonId = invalidId,
            UpdatedFirstName = "Jane",
            UpdatedLastName = "Smith",
            UpdatedAliasName = "JS",
            UpdatedIsInactive = true,
        };

        _personRepositoryMock
            .Setup(r => r.GetPersonByIdAsync(invalidId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Person?)null);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.Not.Null);
        });

        _personRepositoryMock.Verify(
            r => r.GetPersonByIdAsync(invalidId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
