using HomeNet.Core.Common;
using HomeNet.Core.Common.Errors;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Enums;
using HomeNet.Core.Modules.Finances.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Entities;
using HomeNet.Infrastructure.Persistence.Modules.Finances.Extensions;
using Microsoft.Extensions.Logging;
using SqlKata;
using SqlKata.Execution;

namespace HomeNet.Infrastructure.Persistence.Modules.Finances;

public sealed class TransactionRepository : SqlKataRepository,  ITransactionRepository
{
    private static readonly string CategoriesTableName = "finances.categories";
    private static readonly string TransactionsTableName = "finances.transactions";

    private readonly ILogger _logger;

    public TransactionRepository(
        ILogger<TransactionRepository> logger,
        PostgresQueryFactory db)
        : base(db)
    {
        _logger = logger;
    }

    public async Task<IReadOnlyList<Transaction>> GetTransactionsWithCategoryIdAsync(
        int categoryId, CancellationToken cancellationToken = default)
    {
        var query = new Query(TransactionsTableName)
            .Where("category_id", categoryId);
        
        var entities = await GetMultipleAsync<TransactionEntity>(query, cancellationToken);

        return entities
            .Select(e => e.ToTransaction())
            .ToList();
    }

    public async Task<Result> AddTransactionAsync(
        Transaction transaction, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Inserting new transaction");
            var query = new Query(TransactionsTableName).AsInsert(new
            {
                date = transaction.Date,
                amount = transaction.Amount,
                type = transaction.Type,
                category_id = transaction.CategoryId,
            });

            var transactionId = await InsertAndReturnIdAsync(query);
            transaction.Id = transactionId;

            _logger.LogInformation("Transaction inserted successfully with ID: {TransactionId}", transactionId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding transaction with amount: {Amount}", transaction.Amount);
            return new DatabaseError(TransactionsTableName, ex).ToFailure();
        }
    }

    public async Task<MonthlySummary> GetMonthlySummaryAsync(
        int year, int month, CancellationToken cancellationToken = default)
    {
        var transactions = (await _db.Query($"{TransactionsTableName} as t")
            .Join($"{CategoriesTableName} as c", "c.id", "t.category_id")
            .WhereRaw("DATE_TRUNC('month', t.date) = ?", new DateTime(year, month, 1))
            .Select("t.*", "c.name as category_name")
            .OrderBy("t.date")
            .GetAsync<TransactionEntity>(cancellationToken: cancellationToken))
            .ToList();

        var byCategory = transactions
            .GroupBy(t => new { t.CategoryId, t.CategoryName, t.Type })
            .Select(g => new CategorySummary
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.CategoryName ?? string.Empty,
                TransactionType = g.Key.Type,
                TransactionCount = g.Count(),
                TotalAmount = new Money(g.Sum(t => t.Amount)),
            })
            .OrderByDescending(c => c.TotalAmount)
            .ToList();
        
        var totalIncome = transactions
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);
        
        var totalExpense = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);
        
        return new MonthlySummary
        {
            Year = year,
            Month = month,
            TotalIncome = new Money(totalIncome),
            TotalExpenses = new Money(totalExpense),
            ByCategory = byCategory,
        };
    }
}
