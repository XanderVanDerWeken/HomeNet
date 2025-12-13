using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Queries;

public sealed class IncomesQueryHandler : IQueryHandler<IncomesQuery, IReadOnlyList<Income>>
{
    private readonly ITransactionRepository _transactionRepository;

    public IncomesQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<IReadOnlyList<Income>>> HandleAsync(IncomesQuery query, CancellationToken cancellationToken = default)
    {
        var incomes = await _transactionRepository.GetAllIncomesAsync(
            query.Year,
            query.Month,
            cancellationToken);
        
        return Result<IReadOnlyList<Income>>.Success(incomes);
    }
}
