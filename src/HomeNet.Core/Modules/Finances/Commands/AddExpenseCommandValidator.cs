using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed class AddExpenseCommandValidator : BaseValidator<AddExpenseCommand>
{
    protected override void ValidateInternal(AddExpenseCommand entity)
    {
        IsNotEmpty(entity.CategoryName, "Category is required");

        IsNotEmpty(entity.StoreName, "Store Name is required");
    }
}
