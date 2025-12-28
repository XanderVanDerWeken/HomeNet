using HomeNet.Core.Modules.Cards.Abstractions;
using HomeNet.Core.Modules.Cards.Models;
using HomeNet.Core.Modules.Cards.Queries;
using Moq;

namespace HomeNet.Core.Test.Modules.Cards.Queries;

public class CardsExpiringBeforeQueryHandlerTest
{
    private CardsExpiringBeforeQueryHandler _handler;

    private Mock<ICardRepository> _cardRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _cardRepositoryMock = new Mock<ICardRepository>();

        _handler = new CardsExpiringBeforeQueryHandler(_cardRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync()
    {
        // Arrange
        var expiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        var query = new CardsExpiringBeforeQuery
        {
            ExpiryDate = expiryDate,
        };

        var card = new Card
        {
            Id = 1,
            Name = "Test Card",
            ExpirationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)),
        };

        _cardRepositoryMock
            .Setup(x => x.GetAllCardsWithExpiryBeforeAsync(expiryDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { card });

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Has.Count.EqualTo(1));
            Assert.That(result.Value![0], Is.EqualTo(card));
        });

        _cardRepositoryMock.Verify(
            r => r.GetAllCardsWithExpiryBeforeAsync(
                expiryDate,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
