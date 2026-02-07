using HomeNet.Core.Common.Cqrs;

namespace HomeNet.Core.Common.Events;

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
    
    void RegisterCommandHandler<TCommand, TCommandHandler>()
        where TCommand : ICommand
        where TCommandHandler : ICommandHandler<TCommand>;

    void RegisterQueryHandler<TQuery, TQueryHandler, TResult>()
        where TQuery : IQuery
        where TQueryHandler : IQueryHandler<TQuery, TResult>;
    
    void RegisterEventHandler<TEvent, TEventHandler>()
        where TEvent : IEvent
        where TEventHandler : IEventHandler<TEvent>;
}
