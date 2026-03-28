namespace HomeNet.Core.Common.Errors;

// TODO: Use InvalidOp?
public sealed class FinanceBookingError : Error
{
    public FinanceBookingError(string message) 
        : base(ErrorCodes.FinanceBooking, message)
    {
    }
}
