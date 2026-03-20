using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Queries;

public static class AllFixedCosts
{
    public sealed record Query : IQuery
    {
        public bool IncludeInactive { get; init; } = false;
    }

    public sealed class QueryHandler : IQueryHandler<Query, IReadOnlyList<FixedCost>>
    {
        private readonly IFixedCostRepository _fixedCostRepository;

        public QueryHandler(IFixedCostRepository fixedCostRepository)
        {
            _fixedCostRepository = fixedCostRepository;
        }

        public async Task<Result<IReadOnlyList<FixedCost>>> HandleAsync(
            Query query, CancellationToken cancellationToken = default)
        {
            var fixedCosts = await _fixedCostRepository.GetAllFixedCostsAsync(
                query.IncludeInactive, cancellationToken);

            return Result<IReadOnlyList<FixedCost>>.Success(fixedCosts);
        }
    }
}
