using HomeNet.Core.Common.Errors;

namespace HomeNet.Core.Common.Validation;

public static class ResultExtensions
{
    public static Result ToFailure(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            throw new ArgumentNullException("Cannot convert a valid ValidationResult to a failure Result.");
        }

        var validationError = new ValidationError(validationResult.Errors);
        return Result.Failure(validationError);
    }

    public static Result<T> ToFailure<T>(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            throw new ArgumentNullException("Cannot convert a valid ValidationResult to a failure Result.");
        }

        var validationError = new ValidationError(validationResult.Errors);
        return Result<T>.Failure(validationError);
    }
}
