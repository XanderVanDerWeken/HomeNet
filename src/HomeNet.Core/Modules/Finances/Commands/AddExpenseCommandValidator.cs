using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed class AddExpenseCommandValidator : IValidator<AddExpenseCommand>
{
    public ValidationResult Validate(AddExpenseCommand entity)
    {
        List<string> errors = [];

        if (entity.Amount <= 0.0f)
        {
            errors.Add("Amount must be greater than zero");
        }

        if (string.IsNullOrWhiteSpace(entity.CategoryName))
        {
            errors.Add("Category is required");
        }

        if (string.IsNullOrWhiteSpace(entity.StoreName))
        {
            errors.Add("Store Name is required");
        }

        return ValidationResult.FromErrors(errors);
    }
}
