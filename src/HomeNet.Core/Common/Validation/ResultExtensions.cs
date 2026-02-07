namespace HomeNet.Core.Common.Validation;

public static class ResultExtensions
{
    public static Result ToFailure(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            throw new ArgumentNullException("Cannot convert a valid ValidationResult to a failure Result.");
        }

        return Result.Failure(validationResult.ErrorMessage!);
    }

    public static Result<T> ToFailure<T>(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            throw new ArgumentNullException("Cannot convert a valid ValidationResult to a failure Result.");
        }

        return Result<T>.Failure(validationResult.ErrorMessage!);
    }
}
