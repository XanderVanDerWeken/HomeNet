using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Services;

public sealed class TimelineBuilder : ITimelineBuilder
{
    private readonly IMonthlyTimelineRepository _monthlyTimelineRepository;
    private readonly ITransactionRepository _transactionRepository;

    public TimelineBuilder(
        IMonthlyTimelineRepository monthlyTimelineRepository,
        ITransactionRepository transactionRepository)
    {
        _monthlyTimelineRepository = monthlyTimelineRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<MonthlyTimeline> GetOrCreateMonthlyTimelineAsync(
        int year, 
        int month, 
        CancellationToken cancellationToken = default)
    {
        var loadedTimeline = await _monthlyTimelineRepository
            .GetMonthlyTimelineAsync(year, month, cancellationToken);

        if (loadedTimeline != null)
        {
            return loadedTimeline;
        }

        return await RecalculateMonthlyTimelineAsync(year, month, cancellationToken);
    }

    public async Task<MonthlyTimeline> RecalculateMonthlyTimelineAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var builtTimeline = await BuildMonthlyTimelineAsync(year, month, cancellationToken);

        await _monthlyTimelineRepository.SaveMonthlyTimelineAsync(
            builtTimeline, 
            cancellationToken);

        return builtTimeline;
    }

    public async Task<MonthlyTimeline> BuildMonthlyTimelineAsync(
        int year, 
        int month, 
        CancellationToken cancellationToken = default)
    {
        var expenses = await _transactionRepository
            .GetAllExpensesAsync(year, month, cancellationToken);
        
        var incomes = await _transactionRepository
            .GetAllIncomesAsync(year, month, cancellationToken);
        
        var expensesAmount = expenses.Sum(e => e.Amount);
        var incomesAmount = incomes.Sum(e => e.Amount);

        var netTotal = incomesAmount - expensesAmount;

        return new MonthlyTimeline
        {
            Year = year,
            Month = month,
            ExpenseAmount = expensesAmount,
            IncomeAmount = incomesAmount,
            NetTotal = netTotal,
        };
    }
}
