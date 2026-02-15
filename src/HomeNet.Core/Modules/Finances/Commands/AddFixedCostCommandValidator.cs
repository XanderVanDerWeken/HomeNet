using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed class AddFixedCostCommandValidator : BaseValidator<AddFixedCostCommand>
{
    protected override void ValidateInternal(AddFixedCostCommand entity)
    {
        IsNotEmpty(entity.Name, "Fixed cost name must not be empty.");

        IsValidDateInterval(entity.FirstDueDate, entity.LastDueDate, "First due date must be before last due date.");
    }
}
