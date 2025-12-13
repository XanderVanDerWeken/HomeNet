using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed record AddIncomeCommand : ICommand, IValidatable<AddIncomeCommand>
{
    public required float Amount { get; init; }

    public required DateTimeOffset Date { get; init; }

    public required string CategoryName { get; init; }

    public required string Source { get; init; }

    public ValidationResult Validate()
        => new AddIncomeCommandValidator().Validate(this);
}
