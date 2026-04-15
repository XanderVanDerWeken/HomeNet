using HomeNet.Core.Modules.Cards.Models;
using HomeNet.Infrastructure.Persistence.Modules.Cards;
using HomeNet.Infrastructure.Persistence.Modules.Persons;
using HomeNet.Infrastructure.Test.Containers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace HomeNet.Infrastructure.Test.Persistence.Modules.Cards;

public class CardRepositoryTest
{
    private static readonly Card _card1 = new Card
    {
        Name = "My Visa Card",
        ExpirationDate = new DateOnly(2026, 12, 31),
        PersonId = 1,
    };
    private static readonly Card _card2 = new Card
    {
        Name = "My Driver License",
        ExpirationDate = new DateOnly(2030, 6, 30),
        PersonId = 1,
    };

    private CardRepository _cardRepository;

    private PersonRepository _personRepository;
    private static HomenetPgContainer _dbContainer;
    private static CardDbContext _cardDbContext;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        _dbContainer = new HomenetPgContainer();
        await _dbContainer.StartAsync();
        _cardDbContext = _dbContainer.CreateCardDbContext();
        _cardDbContext.Database.Migrate();
        //_dbContainer.InitializeDatabase();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _cardDbContext.DisposeAsync();

        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    [SetUp]
    public async Task Setup()
    {
        _cardRepository = new CardRepository(_cardDbContext);
        _cardDbContext.Database.Migrate();

        var personDbContext = _dbContainer.CreatePersonDbContext();
        _personRepository = new PersonRepository(personDbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _cardDbContext.Cards.RemoveRange(_cardDbContext.Cards);
        _cardDbContext.SaveChanges();
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_AddCardAsync()
    {
        // Act
        var result = await _cardRepository.AddCardAsync(_card1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);
            Assert.That(_card1.Id, Is.EqualTo(1));
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetCardByIdAsync()
    {
        // Arrange
        var card1Added = await _cardRepository.AddCardAsync(_card1);
        var card2Added = await _cardRepository.AddCardAsync(_card2);

        var invalidId = 3;

        // Act
        var card1FromDb = await _cardRepository.GetCardByIdAsync(_card1.Id);
        var card2FromDb = await _cardRepository.GetCardByIdAsync(_card2.Id);
        var cardNonExistent = await _cardRepository.GetCardByIdAsync(invalidId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(card1Added.IsSuccess, Is.True);
            Assert.That(card2Added.IsSuccess, Is.True);

            Assert.That(card1FromDb, Is.Not.Null);
            Assert.That(card1FromDb?.Name, Is.EqualTo(_card1.Name));

            Assert.That(card2FromDb, Is.Not.Null);
            Assert.That(card2FromDb?.Name, Is.EqualTo(_card2.Name));

            Assert.That(cardNonExistent, Is.Null);
        });
    }
}
