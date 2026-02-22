using HomeNet.Core.Modules.Finances.Models;

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

    protected void IsPositiveMoneyAmount(Money money, string errorMessage)
    {
        if (money.Amount < 0)
        {
            Errors.Add(errorMessage);
        }
    }

    protected void IsValidFinanceYear(int year, string errorMessage)
    {
        if (year < 2000 || year > DateTime.Now.Year)
        {
            Errors.Add(errorMessage);
        }
    }

    protected void IsValidFinanceMonth(int month, string errorMessage)
    {
        if (month < 1 || month > 12)
        {
            Errors.Add(errorMessage);
        }
    }
}
