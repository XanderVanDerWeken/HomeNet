using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Queries;

public static class FixedCostHistory
{
    public sealed record Query : IQuery, IValidatable<Query>
    {
        public ValidationResult Validate()
            => new QueryValidator().Validate(this);
    }

    public sealed class QueryHandler : IQueryHandler<Query, IReadOnlyList<FixedCost>>
    {
        public Task<Result<IReadOnlyList<FixedCost>>> HandleAsync(Query query, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class QueryValidator : BaseValidator<Query>
    {
        protected override void ValidateInternal(Query entity)
        {
            throw new NotImplementedException();
        }
    }
}
