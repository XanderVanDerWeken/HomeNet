using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Commands;

public static class UpdateFixedCost
{
    public sealed record Command : ICommand, IValidatable<Command>
    {
        public int FixedCostId { get; init; }

        public required Money Amount { get; init; }

        public DateOnly ValidFrom { get; init; }

        public ValidationResult Validate()
            => new CommandValidator().Validate(this);
    }

    public sealed class CommandHandler : ICommandHandler<Command>
    {
        private readonly IFixedCostRepository _fixedCostRepository;

        public CommandHandler(IFixedCostRepository fixedCostRepository)
        {
            _fixedCostRepository = fixedCostRepository;
        }

        public async Task<Result> HandleAsync(Command command, CancellationToken cancellationToken = default)
        {
            var validationResult = command.Validate();

            if (!validationResult.IsValid)
            {
                return validationResult.ToFailure();
            }

            var deactivationResult = await _fixedCostRepository.DeactivateLastVersionAsync(
                command.FixedCostId, command.ValidFrom.AddDays(-1), cancellationToken);

            if (!deactivationResult.IsSuccess)
            {
                return deactivationResult;
            }

            return await _fixedCostRepository.CreateNewVersionAsync(
                command.FixedCostId, command.Amount, command.ValidFrom, cancellationToken);
        }
    }

    private sealed class CommandValidator : BaseValidator<Command>
    {
        protected override void ValidateInternal(Command entity)
        {
            IsPositiveMoneyAmount(entity.Amount, "Amount must be a positive value.");
        }
    }
}
