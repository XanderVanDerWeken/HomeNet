namespace HomeNet.Core.Common.Validation;

public interface IValidatable<T>
{
    ValidationResult Validate();
}
