using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed record AddFixedCostCommand : ICommand, IValidatable<AddCategoryCommand>
{
    public required string Name { get; init; }

    public required Money Amount { get; init; }

    public required DateOnly FirstDueDate { get; init; }

    public DateOnly? LastDueDate { get; init; } = null;

    public ValidationResult Validate()
        => new AddFixedCostCommandValidator().Validate(this);
}
