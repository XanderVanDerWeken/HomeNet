using HomeNet.Core.Modules.Cards.Abstractions;
using HomeNet.Core.Modules.Cards.Commands;
using HomeNet.Core.Modules.Cards.Models;
using Moq;
using NUnit.Framework;

namespace HomeNet.Core.Test.Modules.Cards.Commands;

public class UpdateCardExpiryCommandHandlerTest
{
    private UpdateCardExpiryCommandHandler _handler;

    private Mock<ICardRepository> _cardRepository;

    [SetUp]
    public void Setup()
    {
        _cardRepository = new Mock<ICardRepository>();

        _handler = new UpdateCardExpiryCommandHandler(_cardRepository.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var originalDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var newDate = originalDate.AddYears(5);

        var card = new Card
        {
            Id = 1,
            Name = "Test Card",
            ExpirationDate = originalDate,
            PersonId = 1,
        };

        var command = new UpdateCardExpiryCommand
        {
            CardId = card.Id,
            NewExpiryDate = newDate,
        };

        _cardRepository
            .Setup(r => r.GetCardByIdAsync(command.CardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(card);
        
        _cardRepository
            .Setup(r => r.UpdateCardAsync(It.IsAny<Card>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Core.Common.Result.Success());

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);
        });

        _cardRepository.Verify(
            r => r.GetCardByIdAsync(command.CardId, It.IsAny<CancellationToken>()),
            Times.Once);
        
        _cardRepository.Verify(
            r => r.UpdateCardAsync(
                It.Is<Card>(c =>
                    c.Id == card.Id &&
                    c.ExpirationDate == newDate),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_CardNotFound()
    {
        // Arrange
        var originalDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var newDate = originalDate.AddYears(5);

        var card = new Card
        {
            Id = 1,
            Name = "Test Card",
            ExpirationDate = originalDate,
            PersonId = 1,
        };

        var command = new UpdateCardExpiryCommand
        {
            CardId = card.Id,
            NewExpiryDate = newDate,
        };

        _cardRepository
            .Setup(r => r.GetCardByIdAsync(command.CardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Card?)null);
        
        _cardRepository
            .Setup(r => r.UpdateCardAsync(It.IsAny<Card>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Core.Common.Result.Success());

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.Not.Null);
        });

        _cardRepository.Verify(
            r => r.GetCardByIdAsync(command.CardId, It.IsAny<CancellationToken>()),
            Times.Once);
        
        _cardRepository.Verify(
            r => r.UpdateCardAsync(
                It.IsAny<Card>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
