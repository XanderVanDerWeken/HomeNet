using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed class AddCategoryCommandValidator : IValidator<AddCategoryCommand>
{
    public ValidationResult Validate(AddCategoryCommand entity)
    {
        List<string> errors = [];

        if (string.IsNullOrWhiteSpace(entity.Name))
        {
            errors.Add("Name is required");
        }

        return ValidationResult.FromErrors(errors);
    }
}
