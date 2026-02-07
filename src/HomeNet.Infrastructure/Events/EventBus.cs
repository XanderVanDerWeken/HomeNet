using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Events;
using Microsoft.Extensions.DependencyInjection;

namespace HomeNet.Infrastructure.Events;

public class EventBus : IEventBus
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Dictionary<Type, Type> _commandHandlers = new();
    private readonly Dictionary<Type, Type> _queryHandlers = new();
    private readonly Dictionary<Type, List<Type>> _eventHandlers = new();

    public EventBus(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public EventBus(
        IServiceScopeFactory scopeFactory, 
        Dictionary<Type, Type> commandHandlers,
        Dictionary<Type, Type> queryHandlers,
        Dictionary<Type, List<Type>> eventHandlers)
    {
        _scopeFactory = scopeFactory;
        _commandHandlers = commandHandlers;
        _queryHandlers = queryHandlers;
        _eventHandlers = eventHandlers;
    }

    public async Task<Result> SendAsync(
        ICommand command, 
        CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();

        if (!_commandHandlers.TryGetValue(commandType, out var handlerType))
            return Result.Failure("No command handler registered.");

        using var scope = _scopeFactory.CreateScope();
        var provider = scope.ServiceProvider;

        dynamic handler = provider.GetRequiredService(handlerType);
        var result = await handler.HandleAsync(
            (dynamic)command, 
            cancellationToken);
        
        return result;
    }

    public async Task<Result<TResult>> SendAsync<TResult>(
        IQuery query, 
        CancellationToken cancellationToken = default)
    {
        var queryType = query.GetType();

        if (!_queryHandlers.TryGetValue(queryType, out var handlerType))
            return Result<TResult>.Failure("No query handler registered.");

        using var scope = _scopeFactory.CreateScope();
        var provider = scope.ServiceProvider;

        dynamic handler = provider.GetRequiredService(handlerType);
        var result = await handler.HandleAsync(
            (dynamic)query, 
            cancellationToken);
        
        return result;
    }

    public async Task PublishAsync(
        IEvent @event,
        CancellationToken cancellationToken = default)
    {
        var eventType = @event.GetType();

        if (!_eventHandlers.TryGetValue(eventType, out var handlerTypes))
            return;

        using var scope = _scopeFactory.CreateScope();
        var provider = scope.ServiceProvider;

        foreach (var handlerType in handlerTypes)
        {
            dynamic handler = provider.GetRequiredService(handlerType);
            await handler.HandleAsync(
                (dynamic)@event, 
                cancellationToken);
        }
    }
    
    public void RegisterCommandHandler<TCommand, TCommandHandler>()
        where TCommand : ICommand
        where TCommandHandler : ICommandHandler<TCommand>
    {
        var commandType = typeof(TCommand);

        if (_commandHandlers.ContainsKey(commandType))
        {
            throw new InvalidOperationException(
                $"A handler for command '{commandType.Name}' is already registered.");
        }

        _commandHandlers[commandType] = typeof(TCommandHandler);
    }

    public void RegisterQueryHandler<TQuery, TQueryHandler, TResult>() 
        where TQuery : IQuery
        where TQueryHandler : IQueryHandler<TQuery, TResult>
    {
        var queryType = typeof(TQuery);

        if (_queryHandlers.ContainsKey(queryType))
        {
            throw new InvalidOperationException(
                $"A handler for query '{queryType.Name}' is already registered.");
        }

        _queryHandlers[queryType] = typeof(TQueryHandler);
    }

    public void RegisterEventHandler<TEvent, TEventHandler>()
        where TEvent : IEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var eventType = typeof(TEvent);

        if (!_eventHandlers.TryGetValue(eventType, out var list))
        {
            list = [];
            _eventHandlers[eventType] = list;
        }

        list.Add(typeof(TEventHandler));
    }
}
