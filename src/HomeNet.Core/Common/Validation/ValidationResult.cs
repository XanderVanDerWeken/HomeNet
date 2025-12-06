namespace HomeNet.Core.Common.Validation;

public sealed record ValidationResult
{
    public required IReadOnlyList<ValidationError> Errors { get; init; }

    public bool IsValid => !Errors.Any();

    public static ValidationResult FromErrors(IEnumerable<ValidationError> errors)
    {
        return new ValidationResult
        {
            Errors = [.. errors]
        };
    }
}
