using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Infrastructure.Events;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace HomeNet.Infrastructure.Test.Events;

public class EventBusTest
{
    private EventBus _eventBus;

    private ServiceCollection _serviceCollection;
    private Mock<IServiceScopeFactory> _scopeFactoryMock;
    private Mock<IServiceScope> _serviceScopeMock;

    [SetUp]
    public void Setup()
    {
        _serviceCollection = new ServiceCollection();
        _serviceScopeMock = new Mock<IServiceScope>();
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();

        _scopeFactoryMock
            .Setup(f => f.CreateScope())
            .Returns(_serviceScopeMock.Object);

        _eventBus = new EventBus(_scopeFactoryMock.Object);
    }

    [Test]
    public async Task Should_SendAsync_CallsCommandHandler()
    {
        // Arrange
        var command = new TestCommand();
        var testCommandHandler = new TestCommandHandler();

        _serviceCollection.AddSingleton<TestCommandHandler>(testCommandHandler);
        
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        _serviceScopeMock
            .Setup(s => s.ServiceProvider)
            .Returns(serviceProvider);

        _eventBus.RegisterCommandHandler(testCommandHandler);

        // Act
        var result = await _eventBus.SendAsync(command);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task Should_SendAsync_CallsQueryHandler()
    {
        // Arrange
        var query = new TestQuery();
        var expectedResult = 42;
        var testQueryHandler = new TestQueryHandler(expectedResult);
        _serviceCollection.AddSingleton<TestQueryHandler>(testQueryHandler);
        
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        _serviceScopeMock
            .Setup(s => s.ServiceProvider)
            .Returns(serviceProvider);

        _eventBus.RegisterQueryHandler(testQueryHandler);

        // Act
        var result = await _eventBus.SendAsync<int>(query);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(expectedResult));
        });
    }

    [Test]
    public async Task Should_PublishAsync_CallsEventHandler()
    {
        // Arrange
        var @event = new TestEvent();
        var testEventHandler = new TestEventHandler();

        _serviceCollection.AddSingleton<TestEventHandler>(testEventHandler);

        var serviceProvider = _serviceCollection.BuildServiceProvider();
        _serviceScopeMock
            .Setup(s => s.ServiceProvider)
            .Returns(serviceProvider);

        _eventBus.RegisterEventHandler(testEventHandler);

        // Act
        await _eventBus.PublishAsync(@event);
    }

    [Test]
    public async Task Should_PublishAsync_CallsMultipleEventHandlers()
    {
        // Arrange
        var @event = new TestEvent();
        var testEventHandler1 = new TestEventHandler();
        var testEventHandler2 = new OtherTestEventHandler();
        
        _serviceCollection.AddSingleton<TestEventHandler>(testEventHandler1);
        _serviceCollection.AddSingleton<OtherTestEventHandler>(testEventHandler2);
        
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        _serviceScopeMock
            .Setup(s => s.ServiceProvider)
            .Returns(serviceProvider);

        _eventBus.RegisterEventHandler(testEventHandler1);
        _eventBus.RegisterEventHandler(testEventHandler2);

        // Act
        await _eventBus.PublishAsync(@event);
    }

    [Test]
    public async Task Should_SendAsync_ThrowsNoHandlerRegistered()
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
    public async Task Should_PublishAsync_WithoutRegisteredHandler()
    {
        // Arrange
        var @event = new TestEvent();

        // Act & Assert
        Assert.DoesNotThrowAsync(async () =>
        {
            await _eventBus.PublishAsync(@event);
        });
    }

    [Test]
    public void Should_RegisterCommandHandler_ThrowsCannotRegisterMultipeHandlersForSameCommand()
    {
        // Arrange
        var testCommandHandlerMock1 = new Mock<ICommandHandler<TestCommand>>();
        var testCommandHandlerMock2 = new Mock<ICommandHandler<TestCommand>>();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            _eventBus.RegisterCommandHandler(testCommandHandlerMock1.Object);
        });
        Assert.Throws<InvalidOperationException>(() =>
        {
            _eventBus.RegisterCommandHandler(testCommandHandlerMock2.Object);
        });
    }

    [Test]
    public void Should_RegisterQueryHandler_ThrowsCannotRegisterMultipeHandlersForSameQuery()
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

    public class TestCommand : ICommand;

    public class TestQuery : IQuery;

    public class TestEvent : IEvent;

    public class TestCommandHandler : ICommandHandler<TestCommand>
    {
        public Task<Result> HandleAsync(
            TestCommand command, 
            CancellationToken cancellationToken = default)
            => Result.Success();
    }

    public class TestQueryHandler : IQueryHandler<TestQuery, int>
    {
        private readonly int _expectedResult;

        public TestQueryHandler(int expectedResult = 42)
        {
            _expectedResult = expectedResult;
        }

        public Task<Result<int>> HandleAsync(
            TestQuery query, 
            CancellationToken cancellationToken = default)
            => Result<int>.Success(_expectedResult);
    }

    public class TestEventHandler : IEventHandler<TestEvent>
    {
        public Task HandleAsync(
            TestEvent @event, 
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    public class OtherTestEventHandler : IEventHandler<TestEvent>
    {
        public Task HandleAsync(
            TestEvent @event, 
            CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
