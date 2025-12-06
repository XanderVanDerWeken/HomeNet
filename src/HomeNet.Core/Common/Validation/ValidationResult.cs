namespace HomeNet.Core.Common.Validation;

public sealed record ValidationResult
{
    public required IReadOnlyList<string> Errors { get; init; }

    public bool IsValid => !Errors.Any();

    public string? ErrorMessage => IsValid ? null : string.Join("; ", Errors);

    public static ValidationResult FromErrors(IEnumerable<string> errors)
    {
        return new ValidationResult
        {
            Errors = [.. errors]
        };
    }
}
