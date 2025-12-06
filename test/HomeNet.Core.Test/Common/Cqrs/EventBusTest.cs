using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using Moq;

namespace HomeNet.Core.Test.Common.Cqrs;

public class EventBusTest
{
    private EventBus _eventBus;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
    }

    [Test]
    public async Task SendAsync_ShouldCallCommandHandler()
    {
        // Arrange
        var command = new TestCommand();
        var testHandlerMock = new Mock<ICommandHandler<TestCommand>>();
        testHandlerMock
            .Setup(h => h.HandleAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        
        _eventBus.RegisterCommandHandler(testHandlerMock.Object);

        // Act
        var result = await _eventBus.SendAsync(command);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsSuccess, Is.True);
        testHandlerMock.Verify(h => h.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task SendAsync_ShouldCallMultipleCommandHandlers()
    {
        // Arrange
        var command = new TestCommand();
        var testHandlerMock1 = new Mock<ICommandHandler<TestCommand>>();
        testHandlerMock1
            .Setup(h => h.HandleAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        
        var testHandlerMock2 = new Mock<ICommandHandler<TestCommand>>();
        testHandlerMock2
            .Setup(h => h.HandleAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        
        _eventBus.RegisterCommandHandler(testHandlerMock1.Object);
        _eventBus.RegisterCommandHandler(testHandlerMock2.Object);

        // Act
        var result = await _eventBus.SendAsync(command);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsSuccess, Is.True);
        testHandlerMock1.Verify(h => h.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        testHandlerMock2.Verify(h => h.HandleAsync(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task SendAsync_ShouldCallQueryHandler()
    {
        // Arrange
        var query = new TestQuery();
        var expectedResult = 42;
        var testHandlerMock = new Mock<IQueryHandler<TestQuery, int>>();
        testHandlerMock
            .Setup(h => h.HandleAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<int>.Success(expectedResult));

        _eventBus.RegisterQueryHandler(testHandlerMock.Object);

        // Act
        var result = await _eventBus.SendAsync<int>(query);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(expectedResult));
        });
        testHandlerMock.Verify(h => h.HandleAsync(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task SendAsync_ShouldFail_NoHandlerRegistered()
    {
        // Arrange
        var command = new TestCommand();
        var query = new TestQuery();

        // Act
        var resultSendCommand = await _eventBus.SendAsync(command);
        var resultSendQuery = await _eventBus.SendAsync<int>(query);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resultSendCommand, Is.Not.Null);
            Assert.That(resultSendCommand.IsSuccess, Is.False);
            Assert.That(resultSendCommand.Error, Is.EqualTo("No command handler registered."));

            Assert.That(resultSendQuery, Is.Not.Null);
            Assert.That(resultSendQuery.IsSuccess, Is.False);
            Assert.That(resultSendQuery.Error, Is.EqualTo("No query handler registered."));
        });
    }

    [Test]
    public void RegisterQueryHandler_CannotRegisterMultipeHandlersForSameQuery()
    {
        // Arrange
        var testQueryHandlerMock1 = new Mock<IQueryHandler<TestQuery, int>>();
        var testQueryHandlerMock2 = new Mock<IQueryHandler<TestQuery, int>>();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            _eventBus.RegisterQueryHandler(testQueryHandlerMock1.Object);
        });
        Assert.Throws<InvalidOperationException>(() =>
        {
            _eventBus.RegisterQueryHandler(testQueryHandlerMock2.Object);
        });
    }

    public class TestCommand : ICommand
    {
    }

    public class TestQuery : IQuery
    {
    }
}
