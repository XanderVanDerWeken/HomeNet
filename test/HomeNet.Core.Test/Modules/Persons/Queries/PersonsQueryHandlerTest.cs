using HomeNet.Core.Modules.Persons.Abstractions;
using HomeNet.Core.Modules.Persons.Models;
using HomeNet.Core.Modules.Persons.Queries;
using Moq;

namespace HomeNet.Core.Test.Modules.Persons.Queries;

public class AllPersonsTest
{
    private AllPersons.QueryHandler _handler;

    private Mock<IPersonRepository> _personRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _personRepositoryMock = new Mock<IPersonRepository>();

        _handler = new AllPersons.QueryHandler(_personRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync()
    {
        // Arrange
        var query = new AllPersons.Query();
        var queryWithInactive = new AllPersons.Query
        {
            IncludeInactivePersons = true,
        };

        var person1 = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            IsInactive = false,
        };

        var person2 = new Person
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Smith",
            IsInactive = true,
        };

        _personRepositoryMock
            .Setup(p => p.GetAllPersonsAsync(true, It.IsAny<CancellationToken>()))
            .ReturnsAsync([person1, person2]);
        
        _personRepositoryMock
            .Setup(p => p.GetAllPersonsAsync(false, It.IsAny<CancellationToken>()))
            .ReturnsAsync([person1]);

        // Act
        var resultOnlyActive = await _handler.HandleAsync(query);
        var resultWithInactive = await _handler.HandleAsync(queryWithInactive);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultOnlyActive.IsSuccess, Is.True);
            Assert.That(resultOnlyActive.Value, Has.Count.EqualTo(1));

            Assert.That(resultOnlyActive.Value![0], Is.EqualTo(person1));

            Assert.That(resultWithInactive.IsSuccess, Is.True);
            Assert.That(resultWithInactive.Value, Has.Count.EqualTo(2));

            Assert.That(resultWithInactive.Value![0], Is.EqualTo(person1));
            Assert.That(resultWithInactive.Value![1], Is.EqualTo(person2));
        });

        _personRepositoryMock.Verify(
            r => r.GetAllPersonsAsync(
                true,
                It.IsAny<CancellationToken>()),
            Times.Once);
        
        _personRepositoryMock.Verify(
            r => r.GetAllPersonsAsync(
                false,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
