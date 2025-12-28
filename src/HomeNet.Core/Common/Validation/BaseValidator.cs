namespace HomeNet.Core.Common.Validation;

public abstract class BaseValidator<T> : IValidator<T>
{
    protected List<string> Errors { get; } = []; 

    public ValidationResult Validate(T entity)
    {
        ValidateInternal(entity);
        return ValidationResult.FromErrors(Errors);
    }

    protected abstract void ValidateInternal(T entity);

    protected void IsNotEmpty(string value, string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Errors.Add(errorMessage);
        }
    }

    protected void IsGreaterThanZero(float value, string errorMessage)
    {
        if (value < 0.0f)
        {
            Errors.Add(errorMessage);
        }
    }
}
