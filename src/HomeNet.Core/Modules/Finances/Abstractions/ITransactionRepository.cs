using HomeNet.Core.Common;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Abstractions;

public interface ITransactionRepository
{
    // TODO: Check if really needed, else with date
    Task<IReadOnlyList<Income>> GetAllIncomesAsync(
        CancellationToken cancellationToken = default);
    
    // TODO: Check if really needed, else with date
    Task<IReadOnlyList<Expense>> GetAllExpensesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Income>> GetAllIncomesAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Expense>> GetAllExpensesAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<Result> AddExpenseAsync(
        Expense expense,
        CancellationToken cancellationToken = default);

    Task<Result> AddIncomeAsync(
        Income income,
        CancellationToken cancellationToken = default);
}
