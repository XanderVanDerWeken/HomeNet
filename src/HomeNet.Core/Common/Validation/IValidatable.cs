namespace HomeNet.Core.Common.Validation;

public interface IValidatable<T>
{
    IValidator<T> GetValidator();
}
