namespace HomeNet.Core.Common.Validation;

public sealed record ValidationError
{
    public required string PropertyName { get; init; }

    public required string ErrorMessage { get; init; }
}
