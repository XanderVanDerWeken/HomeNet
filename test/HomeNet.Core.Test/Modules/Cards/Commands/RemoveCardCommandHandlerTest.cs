using HomeNet.Core.Common;
using HomeNet.Core.Modules.Cards.Abstractions;
using HomeNet.Core.Modules.Cards.Commands;
using Moq;

namespace HomeNet.Core.Test.Modules.Cards.Commands;

public class RemoveCardCommandHandlerTest
{
    private RemoveCardCommandHandler _handler;

    private Mock<ICardRepository> _cardRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _cardRepositoryMock = new Mock<ICardRepository>();

        _handler = new RemoveCardCommandHandler(_cardRepositoryMock.Object);
    }

    [Test]
    public async Task Should_HandleAsync_ReturnsSuccess()
    {
        // Arrange
        var cardId = 1;
        var command = new RemoveCardCommand
        {
            CardId = cardId,
        };

        _cardRepositoryMock
            .Setup(r => r.RemoveCardAsync(cardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Error, Is.Null);
        });

        _cardRepositoryMock.Verify(
            r => r.RemoveCardAsync(
                cardId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
