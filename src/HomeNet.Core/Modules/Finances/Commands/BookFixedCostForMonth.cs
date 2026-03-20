using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Enums;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Commands;

public static class BookFixedCostForMonth
{
    public sealed record Command : ICommand, IValidatable<Command>
    {
        public int Year { get; init; }

        public int Month { get; init; }

        public ValidationResult Validate()
            => new CommandValidator().Validate(this);
    }

    public sealed class CommandHandler : ICommandHandler<Command>
    {
        private readonly IFixedCostRepository _fixedCostRepository;
        private readonly ITransactionRepository _transactionRepository;

        public CommandHandler(
            IFixedCostRepository fixedCostRepository,
            ITransactionRepository transactionRepository)
        {
            _fixedCostRepository = fixedCostRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<Result> HandleAsync(Command command, CancellationToken cancellationToken = default)
        {
            var validationResult = command.Validate();

            if (!validationResult.IsValid)
            {
                return validationResult.ToFailure();
            }

            var fixedCostsWithVersions = await _fixedCostRepository.GetAllFixedCostsWithVersionsInMonthAsync(
                command.Year, command.Month, cancellationToken);
            
            if (fixedCostsWithVersions.Count == 0)
            {
                return Result.Success();
            }
            
            var period = new DateOnly(command.Year, command.Month, 1);
            var fixedCostTransactions = await _fixedCostRepository.GetFixedCostTransactionWithPeriodAsync(
                period, cancellationToken); 
            
            var fixedCostToCreate = fixedCostsWithVersions
                .Where(fcv => !fixedCostTransactions.Any(fct => fcv.FixedCostId == fct.FixedCostId));
            
            foreach (var fc in fixedCostToCreate)
            {
                var transaction = new Transaction
                {
                    CategoryId = fc.CategoryId,
                    Amount = fc.Amount,
                    Type = TransactionType.Expense,
                    Source = TransactionSource.FixedCost,
                    Description = fc.Name,
                    Date = new DateOnly(command.Year, command.Month, fc.DayOfMonth)
                };

                var addTransactionResult = await _transactionRepository.AddTransactionAsync(transaction, cancellationToken);

                if (!addTransactionResult.IsSuccess)
                {
                    return;
                }

                var fixedCostTransaction = new FixedCostTransaction
                {
                    FixedCostId = fc.FixedCostId,
                    FixedCostVersionId = fc.FixedCostVersionId,
                    TransactionId = transaction.Id,
                    Period = period
                };

                await _fixedCostRepository.AddFixedCostTransactionAsync(fixedCostTransaction, cancellationToken);
            }

            return Result.Success();
        }
    }

    private sealed class CommandValidator : BaseValidator<Command>
    {
        protected override void ValidateInternal(Command entity)
        {
            IsValidFinanceYear(entity.Year, "Year must be between 2000 and 2100.");
            IsValidFinanceMonth(entity.Month, "Month must be between 1 and 12.");
        }
    }
}
