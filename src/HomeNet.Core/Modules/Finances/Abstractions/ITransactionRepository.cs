using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Abstractions;

public interface ITransactionRepository
{
    public Task<IReadOnlyList<Transaction>> GetTransactionsWithCategoryIdAsync(
        int categoryId, 
        CancellationToken cancellationToken = default);

    public Task<Result<Transaction>> AddTransactionAsync(
        Transaction transaction, 
        CancellationToken cancellationToken = default);

    public Task<MonthlySummary> GetMonthlySummaryAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);

    public Task<YearlySummary> GetYearlySummaryAsync(
        int year, 
        CancellationToken cancellationToken = default);
}
