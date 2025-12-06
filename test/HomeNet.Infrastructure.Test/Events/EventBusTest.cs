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
    public async Task SendAsync_ShouldCallCommandHandler()
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
    public async Task SendAsync_ShouldCallMultipleCommandHandlers()
    {
        // Arrange
        var command = new TestCommand();
        var testCommandHandler1 = new TestCommandHandler();
        var testCommandHandler2 = new OtherTestCommandHandler();
        
        _serviceCollection.AddSingleton<TestCommandHandler>(testCommandHandler1);
        _serviceCollection.AddSingleton<OtherTestCommandHandler>(testCommandHandler2);
        
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        _serviceScopeMock
            .Setup(s => s.ServiceProvider)
            .Returns(serviceProvider);

        _eventBus.RegisterCommandHandler(testCommandHandler1);
        _eventBus.RegisterCommandHandler(testCommandHandler2);

        // Act
        var result = await _eventBus.SendAsync(command);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsSuccess, Is.True);
    }

    [Test]
    public async Task SendAsync_ShouldCallQueryHandler()
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

    public class TestCommandHandler : ICommandHandler<TestCommand>
    {
        public Task<Result> HandleAsync(
            TestCommand command, 
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Result.Success());
        }
    }

    public class OtherTestCommandHandler : ICommandHandler<TestCommand>
    {
        public Task<Result> HandleAsync(
            TestCommand command, 
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Result.Success());
        }
    }

    public class TestQuery : IQuery
    {
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
        {
            return Task.FromResult(Result<int>.Success(_expectedResult));
        }
    }
}
