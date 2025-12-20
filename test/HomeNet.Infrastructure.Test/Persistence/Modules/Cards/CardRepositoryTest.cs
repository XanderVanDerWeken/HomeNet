using HomeNet.Core.Modules.Cards.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Cards;
using HomeNet.Infrastructure.Test.Containers;
using Npgsql;
using SqlKata.Compilers;

namespace HomeNet.Infrastructure.Test.Persistence.Modules.Cards;

public class CardRepositoryTest
{
    private static readonly Card _card1 = new Card
    {
        Name = "My Visa Card",
        ExpirationDate = new DateOnly(2026, 12, 31),
    };
    private static readonly Card _card2 = new Card
    {
        Name = "My Driver License",
        ExpirationDate = new DateOnly(2030, 6, 30),
    };

    private CardRepository _cardRepository;

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

        _cardRepository = new CardRepository(db);
    }

    [TearDown]
    public async Task Teardown()
    {
        _cardRepository.Dispose();

        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_AddCardAsync()
    {
        // Arrange

        // Act
        var result = await _cardRepository.AddCardAsync(_card1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetCardByIdAsync()
    {
        // Arrange
        var card1Added = await _cardRepository.AddCardAsync(_card1);
        var card2Added = await _cardRepository.AddCardAsync(_card2);

        // Act
        var card1FromDb = await _cardRepository.GetCardByIdAsync(1);
        var card2FromDb = await _cardRepository.GetCardByIdAsync(2);
        var cardNonExistent = await _cardRepository.GetCardByIdAsync(3);

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

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetAllCardAsync()
    {
        // Arrange
        var card1Added = await _cardRepository.AddCardAsync(_card1);
        var card2Added = await _cardRepository.AddCardAsync(_card2);

        // Act
        var cards = await _cardRepository.GetAllCardsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(card1Added.IsSuccess, Is.True);
            Assert.That(card2Added.IsSuccess, Is.True);

            Assert.That(cards, Is.Not.Null);
            Assert.That(cards, Has.Count.EqualTo(2));
            Assert.That(cards.Any(c => c.Name == _card1.Name), Is.True);
            Assert.That(cards.Any(c => c.Name == _card2.Name), Is.True);
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_GetAllCardsWithExpiryBeforeAsync()
    {
        // Arrange
        var card1Added = await _cardRepository.AddCardAsync(_card1);
        var card2Added = await _cardRepository.AddCardAsync(_card2);

        var dateBeforeAll = _card1.ExpirationDate.AddDays(-5);
        var dateAfterFirst = _card1.ExpirationDate.AddMonths(2);
        var dateAfterAll = _card2.ExpirationDate.AddYears(1);

        // Act
        var resultBeforeAll = await _cardRepository.GetAllCardsWithExpiryBeforeAsync(dateBeforeAll);
        var resultAfterFirst = await _cardRepository.GetAllCardsWithExpiryBeforeAsync(dateAfterFirst);
        var resultAfterAll = await _cardRepository.GetAllCardsWithExpiryBeforeAsync(dateAfterAll);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(card1Added.IsSuccess, Is.True);
            Assert.That(card2Added.IsSuccess, Is.True);

            Assert.That(resultBeforeAll, Is.Not.Null);
            Assert.That(resultBeforeAll, Has.Count.EqualTo(0));

            Assert.That(resultAfterFirst, Is.Not.Null);
            Assert.That(resultAfterFirst, Has.Count.EqualTo(1));
            Assert.That(resultAfterFirst.Any(c => c.Name == _card1.Name), Is.True);

            Assert.That(resultAfterAll, Is.Not.Null);
            Assert.That(resultAfterAll, Has.Count.EqualTo(2));
            Assert.That(resultAfterAll.Any(c => c.Name == _card1.Name), Is.True);
            Assert.That(resultAfterAll.Any(c => c.Name == _card2.Name), Is.True);
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_UpdateCardAsync()
    {
        // Arrange
        var card1Added = await _cardRepository.AddCardAsync(_card1);
        var card2Added = await _cardRepository.AddCardAsync(_card2);

        var card1NewName = "UpdatedCard";

        var card1FromDb = await _cardRepository.GetCardByIdAsync(1);

        // Act
        _card1.Name = card1NewName;
        _card1.Id = 1; // TODO: Need to handle card id
        var card1UpdateResult = await _cardRepository.UpdateCardAsync(_card1);

        var updatedCard1FromDb = await _cardRepository.GetCardByIdAsync(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(card1Added.IsSuccess, Is.True);
            Assert.That(card2Added.IsSuccess, Is.True);

            Assert.That(card1UpdateResult.IsSuccess, Is.True);
            Assert.That(card1UpdateResult.Error, Is.Null);

            Assert.That(card1FromDb, Is.Not.Null);
            Assert.That(updatedCard1FromDb, Is.Not.Null);

            Assert.That(card1FromDb?.Name, Is.EqualTo("My Visa Card"));
            Assert.That(updatedCard1FromDb?.Name, Is.EqualTo(card1NewName));
            Assert.That(updatedCard1FromDb?.Name, Is.Not.EqualTo(card1FromDb?.Name));
        });
    }

    [Test]
    [Explicit("Needs Docker running")]
    public async Task Should_RemoveCardAsync()
    {
        // Arrange
        await _cardRepository.AddCardAsync(_card1);
        await _cardRepository.AddCardAsync(_card2);

        var cardsBeforeDelete = await _cardRepository.GetAllCardsAsync();

        // Act
        var deleteResult = await _cardRepository.RemoveCardAsync(1);

        var cardsAfterDelete = await _cardRepository.GetAllCardsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(deleteResult.IsSuccess, Is.True);
            Assert.That(deleteResult.Error, Is.Null);

            Assert.That(cardsBeforeDelete, Has.Count.EqualTo(2));
            Assert.That(cardsAfterDelete, Has.Count.EqualTo(1));
            Assert.That(cardsAfterDelete.Any(c => c.Id == 1), Is.False);
            Assert.That(cardsAfterDelete.Any(c => c.Name == _card2.Name), Is.True);
        });
    }
}
