using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Errors;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed class AddExpenseCommandHandler : ICommandHandler<AddExpenseCommand>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;

    public AddExpenseCommandHandler(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result> HandleAsync(
        AddExpenseCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = command.Validate();

        if (!validationResult.IsValid)
        {
            return validationResult.ToFailure();
        }

        var foundCategory = await _categoryRepository.GetCategoryByNameAsync(
            command.CategoryName,
            cancellationToken);

        if (foundCategory == null)
        {
            return new NotFoundError("Category", command.CategoryName).ToFailure();
        }

        var newExpense = new Expense
        {
            Amount = command.Amount,
            Date = command.Date,
            Category = foundCategory,
            Store = command.StoreName,
        };

        return await _transactionRepository.AddExpenseAsync(
            newExpense, 
            cancellationToken);
    }
}
