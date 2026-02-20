namespace HomeNet.Core.Common.Errors;

public sealed class ValidationError : Error
{
    public IReadOnlyList<string> Failures { get; }

    public ValidationError(IEnumerable<string> failures)
        : base(ErrorCodes.Validation, "One or more validation errors occurred.")
    {
        Failures = failures.ToList();
    }
}
