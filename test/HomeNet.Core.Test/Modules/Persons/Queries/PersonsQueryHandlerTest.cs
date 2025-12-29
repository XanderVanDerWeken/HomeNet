using HomeNet.Core.Modules.Persons.Abstractions;
using HomeNet.Core.Modules.Persons.Models;
using HomeNet.Core.Modules.Persons.Queries;
using Moq;

namespace HomeNet.Core.Test.Modules.Persons.Queries;

public class PersonsQueryHandlerTest
{
    private PersonsQueryHandler _handler;

    private Mock<IPersonRepository> _personRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _personRepositoryMock = new Mock<IPersonRepository>();

        _handler = new PersonsQueryHandler(_personRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync()
    {
        // Arrange
        var query = new PersonsQuery();

        var person1 = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
        };

        var person2 = new Person
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Smith",
        };

        _personRepositoryMock
            .Setup(p => p.GetAllPersonsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([person1, person2]);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Has.Count.EqualTo(2));

            Assert.That(result.Value![0], Is.EqualTo(person1));
            Assert.That(result.Value![1], Is.EqualTo(person2));
        });

        _personRepositoryMock.Verify(
            r => r.GetAllPersonsAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
