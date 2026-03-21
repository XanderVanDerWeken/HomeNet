namespace HomeNet.Core.Common.Errors;

public sealed class FinanceBookingError : Error
{
    public FinanceBookingError(string message) 
        : base(ErrorCodes.FinanceBooking, message)
    {
    }
}
