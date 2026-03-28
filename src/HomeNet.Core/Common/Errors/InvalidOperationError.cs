namespace HomeNet.Core.Common.Errors;

public sealed class InvalidOperationError : Error
{
    public InvalidOperationError(string message) 
        : base(ErrorCodes.InvalidOperation, message)
    {
    } 
}
