using System.Data;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Abstractions;

public interface IFixedCostTransactionRepository
{
    public Task AddFixedCostTransactionAsync(
        FixedCostTransaction fixedCostTransaction, 
        IDbTransaction dbTransaction,
        CancellationToken cancellationToken = default);
    
    public Task AddTransactionAsync(
        Transaction transaction,
        IDbTransaction dbTransaction,
        CancellationToken cancellationToken = default);
}
