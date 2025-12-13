using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed class AddCategoryCommandValidator : BaseValidator<AddCategoryCommand>
{
    protected override void ValidateInternal(AddCategoryCommand entity)
    {
        IsNotEmpty(entity.Name, "Category name must not be empty.");
    }
}
