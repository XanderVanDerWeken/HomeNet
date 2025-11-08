namespace HomeNet.Core.Events;

public class EventBus : IEventBus
{
    private readonly Dictionary<Type, List<object>> _handlers = new();

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent
    {
        var type = typeof(TEvent);

        if (!_handlers.TryGetValue(type, out var handlers))
        {
            return;
        }

        foreach (var handlerObject in handlers)
        {
            if (handlerObject is IEventHandler<TEvent> handler)
            {
                await handler.HandleAsync(@event);
            }
        }
    }

    public void Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : IDomainEvent
    {
        var type = typeof(TEvent);

        if (!_handlers.ContainsKey(type))
        {
            _handlers[type] = [];
        }

        _handlers[type].Add(handler);
    }
}
