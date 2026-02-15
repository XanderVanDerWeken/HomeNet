using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Queries;

public sealed class FixedCostsQueryHandler : IQueryHandler<FixedCostsQuery, IReadOnlyList<FixedCost>>
{
    private readonly IFixedCostRepository _fixedCostRepository;

    public FixedCostsQueryHandler(IFixedCostRepository fixedCostRepository)
    {
        _fixedCostRepository = fixedCostRepository;
    }

    public async Task<Result<IReadOnlyList<FixedCost>>> HandleAsync(FixedCostsQuery query, CancellationToken cancellationToken = default)
    {
        var fixedCosts = await _fixedCostRepository
            .GetAllFixedCostsAsync(cancellationToken);

        return Result<IReadOnlyList<FixedCost>>.Success(fixedCosts);
    }
}
