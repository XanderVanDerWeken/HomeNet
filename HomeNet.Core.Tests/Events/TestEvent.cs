namespace HomeNet.Core.Tests.Events;

using HomeNet.Core.Events;

public record TestEvent : IDomainEvent
{
    public required string Message { get; init; }
}
