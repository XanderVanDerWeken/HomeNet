using HomeNet.Core.Modules.Cards.Abstractions;
using HomeNet.Core.Modules.Cards.Commands;
using HomeNet.Core.Modules.Cards.Models;
using Moq;

namespace HomeNet.Core.Test.Modules.Cards.Commands;

public class AddCardTest
{
    private AddCard.CommandHandler _handler;

    private Mock<ICardRepository> _cardRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _cardRepositoryMock = new Mock<ICardRepository>();

        _handler = new AddCard.CommandHandler(_cardRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var command = new AddCard.Command
        {
            Name = "Test Card",
            ExpirationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
            PersonId = 1,
        };

        _cardRepositoryMock
            .Setup(r => r.AddCardAsync(It.IsAny<Card>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Core.Common.Result.Success());

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);
        });

        _cardRepositoryMock.Verify(
            r => r.AddCardAsync(
                It.Is<Card>(c =>
                    c.Name == command.Name &&
                    c.ExpirationDate == command.ExpirationDate),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsFailure_InvalidCommand()
    {
        // Arrange
        var commandInvalidName = new AddCard.Command
        {
            Name = string.Empty,
            ExpirationDate = DateOnly.FromDateTime(DateTime.UtcNow),
            PersonId = 1,
        };
        var commandInvalidDate = new AddCard.Command
        {
            Name = "Valid Name",
            ExpirationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6)),
            PersonId = 1,
        };

        _cardRepositoryMock
            .Setup(r => r.AddCardAsync(It.IsAny<Card>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Core.Common.Result.Success());

        // Act
        var resultInvalidName = await _handler.HandleAsync(commandInvalidName);
        var resultInvalidDate = await _handler.HandleAsync(commandInvalidDate);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultInvalidName.IsSuccess, Is.False);
            Assert.That(resultInvalidName.Error, Is.Not.Null);

            Assert.That(resultInvalidDate.IsSuccess, Is.False);
            Assert.That(resultInvalidDate.Error, Is.Not.Null);
        });

        _cardRepositoryMock.Verify(
            r => r.AddCardAsync(
                It.IsAny<Card>(), 
                It.IsAny<CancellationToken>()), 
            Times.Never);
    }
}
