namespace HomeNet.Core.Common.Validation;

public interface IValidator<T>
{
    ValidationResult Validate(T entity);
}
