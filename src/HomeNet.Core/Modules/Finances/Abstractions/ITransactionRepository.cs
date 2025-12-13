using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Abstractions;

public interface ITransactionRepository
{
    Task<IReadOnlyList<Income>> GetAllIncomesAsync(
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Expense>> GetAllExpensesAsync(
        CancellationToken cancellationToken = default);

    Task<Result> AddExpenseAsync(
        Expense expense,
        CancellationToken cancellationToken = default);

    Task<Result> AddIncomeAsync(
        Income income,
        CancellationToken cancellationToken = default);
}
