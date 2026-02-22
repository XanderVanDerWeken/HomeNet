using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Abstractions;

public interface ITransactionRepository
{
    public Task<IReadOnlyList<Transaction>> GetTransactionsWithCategoryIdAsync(
        int categoryId, 
        CancellationToken cancellationToken = default);

    public Task<Result> AddTransactionAsync(
        Transaction transaction, 
        CancellationToken cancellationToken = default);
}
