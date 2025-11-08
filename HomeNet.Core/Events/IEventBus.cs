namespace HomeNet.Core.Events;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event)
        where TEvent : IDomainEvent;

    void Subscribe<TEvent>(IEventHandler<TEvent> handler)
        where TEvent : IDomainEvent;
}
