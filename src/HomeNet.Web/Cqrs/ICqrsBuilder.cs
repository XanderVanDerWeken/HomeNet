using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Events;

namespace HomeNet.Web.Cqrs;

public interface ICqrsBuilder
{
    public void AddCommand<TCommand, THandler>()
        where TCommand : ICommand
        where THandler : class, ICommandHandler<TCommand>;
    
    public void AddQuery<TQuery, THandler, TResult>()
        where TQuery : IQuery
        where THandler : class, IQueryHandler<TQuery, TResult>;
    
    public void AddEvent<TEvent, THandler>()
        where TEvent : IEvent
        where THandler : class, IEventHandler<TEvent>;
}
