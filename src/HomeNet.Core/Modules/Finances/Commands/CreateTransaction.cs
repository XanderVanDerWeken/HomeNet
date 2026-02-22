using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Enums;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Commands;

public static class CreateTransaction
{
    public sealed record Command : ICommand, IValidatable<Command>
    {
        public int CategoryId { get; init; }

        public required Money Amount { get; init; }

        public required TransactionType Type { get; init; }

        public TransactionSource Source { get; init; } = TransactionSource.Manual;

        public string? Description { get; init; }

        public required DateOnly Date { get; init; }

        public ValidationResult Validate()
            => new CommandValidator().Validate(this);
    }

    public sealed class CommandHandler : ICommandHandler<Command>
    {
        private readonly ITransactionRepository _transactionRepository;

        public CommandHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        
        public Task<Result> HandleAsync(
            Command command, CancellationToken cancellationToken = default)
        {
            var validationResult = command.Validate();

            if (!validationResult.IsValid)
            {
                return validationResult.ToFailure();
            }

            var newTransaction = new Transaction
            {
                CategoryId = command.CategoryId,
                Amount = command.Amount,
                Type = command.Type,
                Source = command.Source,
                Description = command.Description,
                Date = command.Date,
            };

            return _transactionRepository.AddTransactionAsync(
                newTransaction, cancellationToken);
        }
    }

    private sealed class CommandValidator : BaseValidator<Command>
    {
        protected override void ValidateInternal(Command entity)
        {
            IsGreaterThanZero(entity.CategoryId, "CategoryId must be greater than zero.");
        }
    }
}
