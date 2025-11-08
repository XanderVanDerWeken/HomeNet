namespace HomeNet.Core.Tests.Events;

using HomeNet.Core.Events;

public class EventBusTest
{
    private EventBus _eventBus;
    private TestEvent _testEvent;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _testEvent = new TestEvent { Message = "Test" };
    }

    [Test]
    public void Test_PublishToNoone()
    {
        Assert.DoesNotThrowAsync(async () =>
        {
            await _eventBus.PublishAsync(_testEvent);
        });
    }

    [Test]
    public void Test_PublishToOne()
    {
        // Arrange
        var handler = new TestEventHandler();
        _eventBus.Subscribe(handler);

        // Act
        Assert.DoesNotThrowAsync(async () =>
        {
            await _eventBus.PublishAsync(_testEvent);
        });

        // Assert
        Assert.That(handler.HandleCount, Is.EqualTo(1));
    }

    [Test]
    public void Test_PublishToMultiple()
    {
        // Arrange
        var handler1 = new TestEventHandler();
        _eventBus.Subscribe(handler1);

        var handler2 = new TestEventHandler();
        _eventBus.Subscribe(handler2);

        // Act
        Assert.DoesNotThrowAsync(async () =>
        {
            await _eventBus.PublishAsync(_testEvent);
        });

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(handler1.HandleCount, Is.EqualTo(1));
            Assert.That(handler2.HandleCount, Is.EqualTo(1));
        });
    }
}
