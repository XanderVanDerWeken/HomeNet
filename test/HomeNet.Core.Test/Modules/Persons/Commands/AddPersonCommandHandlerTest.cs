using HomeNet.Core.Modules.Persons.Abstractions;
using HomeNet.Core.Modules.Persons.Commands;
using HomeNet.Core.Modules.Persons.Models;
using Moq;
using NUnit.Framework;

namespace HomeNet.Core.Test.Modules.Persons.Commands;

public class AddPersonCommandHandlerTest
{
    private AddPersonCommandHandler _handler;

    private Mock<IPersonRepository> _personRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _personRepositoryMock = new Mock<IPersonRepository>();

        _handler = new AddPersonCommandHandler(_personRepositoryMock.Object);
    }

    [TestCase("JD")]
    [TestCase(null)]
    public async Task Should_HandleAsync_ReturnsSuccess(string? aliasName)
    {
        // Arrange
        var command = new AddPersonCommand
        {
            FirstName = "John",
            LastName = "Doe",
            AliasName = aliasName,
        };

        _personRepositoryMock
            .Setup(r => r.AddPersonAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()))
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
            r => r.AddPersonAsync(
                It.Is<Person>(p =>
                    p.FirstName == command.FirstName &&
                    p.LastName == command.LastName &&
                    p.AliasName == command.AliasName),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_InvalidCommand()
    {
        // Arrange
        var commandInvalidFirstName = new AddPersonCommand
        {
            FirstName = "",
            LastName = "Doe",
            AliasName = null,
        };
        var commandInvalidLastName = new AddPersonCommand
        {
            FirstName = "John",
            LastName = "",
            AliasName = null,
        };

        _personRepositoryMock
            .Setup(r => r.AddPersonAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Core.Common.Result.Success());

        // Act
        var resultInvalidFirstName = await _handler.HandleAsync(commandInvalidFirstName);
        var resultInvalidLastName = await _handler.HandleAsync(commandInvalidLastName);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultInvalidFirstName.IsSuccess, Is.False);
            Assert.That(resultInvalidFirstName.Error, Is.Not.Null);

            Assert.That(resultInvalidLastName.IsSuccess, Is.False);
            Assert.That(resultInvalidLastName.Error, Is.Not.Null);
        });

        _personRepositoryMock.Verify(
            r => r.AddPersonAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
