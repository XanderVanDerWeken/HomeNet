using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Finances.Queries;

public sealed record IncomesQuery : IQuery, IValidatable<IncomesQuery>
{
    public int Year { get; init; }

    public int Month { get; init; }

    public ValidationResult Validate()
        => new IncomesQueryValidator().Validate(this);
}
