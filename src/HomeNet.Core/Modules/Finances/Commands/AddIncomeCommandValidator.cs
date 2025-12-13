using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed class AddIncomeCommandValidator : BaseValidator<AddIncomeCommand>
{
    protected override void ValidateInternal(AddIncomeCommand entity)
    {
        IsGreaterThanZero(entity.Amount, "Amount must be greater than zero");

        IsNotEmpty(entity.CategoryName, "Category is required");

        IsNotEmpty(entity.Source, "Source is required");
    }
}
