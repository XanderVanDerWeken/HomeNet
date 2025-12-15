using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Finances.Queries;

public sealed record ExpensesQuery : IQuery, IValidatable<ExpensesQuery>
{
    public required int Year { get; init; }

    public required int Month { get; init; }

    public ValidationResult Validate()
        => new ExpensesQueryValidator().Validate(this);
}
