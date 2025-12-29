using HomeNet.Core.Modules.Persons.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Persons;
using HomeNet.Infrastructure.Test.Containers;
using Npgsql;
using SqlKata.Compilers;

namespace HomeNet.Infrastructure.Test.Persistence.Modules.Persons;

public class PersonRepositoryTest
{
    private static readonly Person _person1 = new Person
    {
        FirstName = "John",
        LastName = "Doe",
        AliasName = "JD",
    };

    private static readonly Person _person2 = new Person
    {
        FirstName = "Jane",
        LastName = "Smith",
        AliasName = null,
    };

    private PersonRepository _personRepository;
    
    private HomenetPgContainer _dbContainer;

    [SetUp]
    public async Task Setup()
    {
        _dbContainer = new HomenetPgContainer();
        await _dbContainer.StartAsync();

        var connectionString = _dbContainer.GetConnectionString();

        var connection = new NpgsqlConnection(connectionString);
        var compiler = new PostgresCompiler();

        var db = new PostgresQueryFactory(connection, compiler);

        _personRepository = new PersonRepository(db);
    }

    [TearDown]
    public async Task Teardown()
    {
        _personRepository.Dispose();

        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_AddPersonAsync()
    {
        // Arrange

        // Act
        var resultWithAlias = await _personRepository.AddPersonAsync(_person1);
        var resultWithoutAlias = await _personRepository.AddPersonAsync(_person2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultWithAlias.IsSuccess, Is.True);
            Assert.That(resultWithAlias.Error, Is.Null);
            Assert.That(_person1.Id, Is.EqualTo(1));

            Assert.That(resultWithoutAlias.IsSuccess, Is.True);
            Assert.That(resultWithoutAlias.Error, Is.Null);
            Assert.That(_person2.Id, Is.EqualTo(2));
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetPersonByIdAsync()
    {
        // Arrange
        var invalidId = 999;

        var person1Added = await _personRepository.AddPersonAsync(_person1);
        var person2Added = await _personRepository.AddPersonAsync(_person2);

        // Act
        var resultPerson1 = await _personRepository.GetPersonByIdAsync(_person1.Id);
        var resultPerson2 = await _personRepository.GetPersonByIdAsync(_person2.Id);
        var resultInvalidPerson = await _personRepository.GetPersonByIdAsync(invalidId);


        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(person1Added.IsSuccess, Is.True);
            Assert.That(person2Added.IsSuccess, Is.True);

            Assert.That(resultPerson1, Is.Not.Null);
            Assert.That(resultPerson1!.FirstName, Is.EqualTo(_person1.FirstName));
            Assert.That(resultPerson1!.LastName, Is.EqualTo(_person1.LastName));
            Assert.That(resultPerson1!.AliasName, Is.EqualTo(_person1.AliasName));

            Assert.That(resultPerson2, Is.Not.Null);
            Assert.That(resultPerson2!.FirstName, Is.EqualTo(_person2.FirstName));
            Assert.That(resultPerson2!.LastName, Is.EqualTo(_person2.LastName));
            Assert.That(resultPerson2!.AliasName, Is.EqualTo(_person2.AliasName));

            Assert.That(resultInvalidPerson, Is.Null);
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetAllPersonsAsync()
    {
        // Arrange
        var person1Added = await _personRepository.AddPersonAsync(_person1);
        var person2Added = await _personRepository.AddPersonAsync(_person2);

        // Act
        var result = await _personRepository.GetAllPersonsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(person1Added.IsSuccess, Is.True);
            Assert.That(person2Added.IsSuccess, Is.True);

            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.Any(p => p.Id == _person1.Id), Is.True);
            Assert.That(result.Any(p => p.Id == _person2.Id), Is.True);
        });
    }
}
