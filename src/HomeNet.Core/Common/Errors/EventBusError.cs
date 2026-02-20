namespace HomeNet.Core.Common.Errors;

public sealed class EventBusError : Error
{
    public EventBusError(string message)
        : base(ErrorCodes.EventBus, message)
    {
    }
}
