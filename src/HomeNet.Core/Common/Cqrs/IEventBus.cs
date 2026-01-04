namespace HomeNet.Core.Common.Cqrs;

public interface IEventBus
{
    Task<Result> SendAsync(
        ICommand command, 
        CancellationToken cancellationToken = default);
    
    Task<Result<TResult>> SendAsync<TResult>(
        IQuery query,
        CancellationToken cancellationToken = default);
    
    Task PublishAsync(
        IEvent @event,
        CancellationToken cancellationToken = default);
    
    void RegisterCommandHandler<TCommand>(
        ICommandHandler<TCommand> handler)
        where TCommand : ICommand;

    void RegisterQueryHandler<TQuery, TResult>(
        IQueryHandler<TQuery, TResult> handler)
        where TQuery : IQuery;
    
    void RegisterEventHandler<TEvent>(
        IEventHandler<TEvent> handler)
        where TEvent : IEvent;
}
