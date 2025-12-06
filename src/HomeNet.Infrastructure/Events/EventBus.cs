using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using Microsoft.Extensions.DependencyInjection;

namespace HomeNet.Infrastructure.Events;

public class EventBus : IMediator
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Dictionary<Type, List<Type>> _commandHandlers = new();
    private readonly Dictionary<Type, Type> _queryHandlers = new();

    public EventBus(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<Result> SendAsync(
        ICommand command, 
        CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();

        if (!_commandHandlers.TryGetValue(commandType, out var handlerTypes))
            return Result.Failure("No command handler registered.");

        var totalResult = Result.Success();

        using var scope = _scopeFactory.CreateScope();
        var provider = scope.ServiceProvider;

        foreach (var handlerType in handlerTypes)
        {
            dynamic handler = provider.GetRequiredService(handlerType);
            Result result = await handler.HandleAsync((dynamic)command, cancellationToken);

            if (!result.IsSuccess)
                totalResult = result;
        }

        return totalResult;
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
        var result = await handler.HandleAsync((dynamic)query, cancellationToken);
        return result;
    }
    
    public void RegisterCommandHandler<TCommand>(
        ICommandHandler<TCommand> handler) 
        where TCommand : ICommand
    {
        var commandType = typeof(TCommand);

        if (!_commandHandlers.TryGetValue(commandType, out var list))
        {
            list = [];
            _commandHandlers[commandType] = list;
        }

        var commandHandlerType = handler.GetType();
        list.Add(commandHandlerType);
    }

    public void RegisterQueryHandler<TQuery, TResult>(
        IQueryHandler<TQuery, TResult> handler) 
        where TQuery : IQuery
    {
        var queryType = typeof(TQuery);

        if (_queryHandlers.ContainsKey(queryType))
        {
            throw new InvalidOperationException(
                $"A handler for query '{queryType.Name}' is already registered."
            );
        }

        var queryHandlerType = handler.GetType();
        _queryHandlers[queryType] = queryHandlerType;
    }
}
