using System;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Abstractions;

public interface ITransactionRepository
{
    Task<IReadOnlyList<Income>> GetAllIncomesAsync(
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Expense>> GetAllExpensesAsync(
        CancellationToken cancellationToken = default);
}
