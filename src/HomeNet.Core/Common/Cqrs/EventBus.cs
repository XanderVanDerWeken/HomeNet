namespace HomeNet.Core.Common.Cqrs;

public class EventBus : IMediator
{
    private readonly Dictionary<Type, List<object>> _commandHandlers = new();
    private readonly Dictionary<Type, object> _queryHandlers = new();

    public async Task<Result> SendAsync(
        ICommand command, 
        CancellationToken cancellationToken = default)
    {
        var type = command.GetType();

        if (!_commandHandlers.TryGetValue(type, out var handlers))
            return Result.Failure("No command handler registered.");

        Result finalResult = Result.Success();

        foreach (var handlerObject in handlers)
        {
            dynamic handler = handlerObject;
            Result result = await handler.HandleAsync(
                (dynamic)command, 
                cancellationToken);

            if (!result.IsSuccess)
                finalResult = Result.Failure(result.Error!);
        }

        return finalResult;
    }

    public async Task<Result<TResult>> SendAsync<TResult>(
        IQuery query, 
        CancellationToken cancellationToken = default)
    {
        var type = query.GetType();

        if (!_queryHandlers.TryGetValue(type, out var handlerObj))
            return Result<TResult>.Failure("No query handler registered.");

        dynamic handler = handlerObj;
        return await handler.HandleAsync((dynamic)query, cancellationToken);
    }

    public void RegisterCommandHandler<TCommand>(
        ICommandHandler<TCommand> handler) 
        where TCommand : ICommand
    {
        var type = typeof(TCommand);

        if (!_commandHandlers.ContainsKey(type))
            _commandHandlers[type] = new List<object>();

        _commandHandlers[type].Add(handler);
    }

    public void RegisterQueryHandler<TQuery, TResult>(
        IQueryHandler<TQuery, TResult> handler) 
        where TQuery : IQuery
    {
        var type = typeof(TQuery);

        if (_queryHandlers.ContainsKey(type))
        {
            throw new InvalidOperationException(
                $"A handler for query '{type.Name}' is already registered."
            );
        }

        _queryHandlers[type] = handler;
    }
}
