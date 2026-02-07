using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Events;

namespace HomeNet.Web.Cqrs;

internal sealed class CqrsBuilder : ICqrsBuilder
{
    internal Dictionary<Type, Type> Commands { get; } = new();
    internal Dictionary<Type, Type> Queries { get; } = new();
    internal Dictionary<Type, List<Type>> Events { get; } = new();

    public void AddCommand<TCommand, THandler>()
        where TCommand : ICommand
        where THandler : class, ICommandHandler<TCommand>
        => Commands[typeof(TCommand)] = typeof(THandler);

    public void AddQuery<TQuery, THandler, TResult>()
        where TQuery : IQuery
        where THandler : class, IQueryHandler<TQuery, TResult>
        => Queries[typeof(TQuery)] = typeof(THandler);

    public void AddEvent<TEvent, THandler>()
        where TEvent : IEvent
        where THandler : class, IEventHandler<TEvent>
    {
        var key = typeof(TEvent);
        if (!Events.TryGetValue(key, out var list))
        {
            list = new List<Type>();
            Events[key] = list;
        }
        list.Add(typeof(THandler));
    }
}
