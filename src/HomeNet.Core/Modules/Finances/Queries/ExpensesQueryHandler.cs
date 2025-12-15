using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Queries;

public sealed class ExpensesQueryHandler : IQueryHandler<ExpensesQuery, IReadOnlyList<Expense>>
{
    private readonly ITransactionRepository _transactionRepository;

    public ExpensesQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<IReadOnlyList<Expense>>> HandleAsync(
        ExpensesQuery query, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = query.Validate();

        if (!validationResult.IsValid)
        {
            return Result<IReadOnlyList<Expense>>.Failure(validationResult.ErrorMessage!);
        }

        var expenses = await _transactionRepository.GetAllExpensesAsync(
            query.Year,
            query.Month,
            cancellationToken);
        
        return Result<IReadOnlyList<Expense>>.Success(expenses);
    }
}
