using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Commands;

public sealed class AddIncomeCommandHandler : ICommandHandler<AddIncomeCommand>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;

    public AddIncomeCommandHandler(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result> HandleAsync(
        AddIncomeCommand command, 
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
            return Result.Failure($"Category '{command.CategoryName}' not found.");
        }

        var newIncome = new Income
        {
            Amount = command.Amount,
            Date = command.Date,
            Category = foundCategory,
            Source = command.Source,
        };

        return await _transactionRepository.AddIncomeAsync(
            newIncome, 
            cancellationToken);
    }
}
