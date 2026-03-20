using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Queries;

public static class FixedCostHistory
{
    public sealed record Query : IQuery
    {
        public int FixedCostId { get; init; }
    }

    public sealed class QueryHandler : IQueryHandler<Query, IReadOnlyList<FixedCostVersion>>
    {
        private readonly IFixedCostRepository _fixedCostRepository;

        public QueryHandler(IFixedCostRepository fixedCostRepository)
        {
            _fixedCostRepository = fixedCostRepository;
        }

        public async Task<Result<IReadOnlyList<FixedCostVersion>>> HandleAsync(
            Query query, CancellationToken cancellationToken = default)
        {
            return await _fixedCostRepository.GetFixedCostHistoryAsync(
                query.FixedCostId, cancellationToken);
        }
    }
}
