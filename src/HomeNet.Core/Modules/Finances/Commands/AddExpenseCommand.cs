using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed record AddExpenseCommand : ICommand, IValidatable<AddExpenseCommand>
{
    public required float Amount { get; init; }

    public required DateTimeOffset Date { get; init; }

    public required string CategoryName { get; init; }

    public required string StoreName { get; init; }

    public ValidationResult Validate()
        => new AddExpenseCommandValidator().Validate(this);
}
