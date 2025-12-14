using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed record AddIncomeCommand : ICommand, IValidatable<AddIncomeCommand>
{
    public required Money Amount { get; init; }

    public required DateOnly Date { get; init; }

    public required string CategoryName { get; init; }

    public required string Source { get; init; }

    public ValidationResult Validate()
        => new AddIncomeCommandValidator().Validate(this);
}
