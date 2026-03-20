using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Models;

namespace HomeNet.Core.Modules.Finances.Commands;

public static class CreateFixedCost
{
    public sealed record Command : ICommand, IValidatable<Command>
    {
        public required string Name { get; init; }

        public int CategoryId { get; init; }

        public int DayOfMonth { get; init; }

        public Money InitialAmount { get; init; }

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

        public Task<Result> HandleAsync(
            Command command, CancellationToken cancellationToken = default)
        {
            var validationResult = command.Validate();

            if (!validationResult.IsValid)
            {
                return validationResult.ToFailure();
            }

            var newFixedCost = new FixedCost
            {
                Name = command.Name,
                CategoryId = command.CategoryId,
                DayOfMonth = command.DayOfMonth,
            };

            var newFixedCostVersion = new FixedCostVersion
            {
                Amount = command.InitialAmount,
                ValidFrom = command.ValidFrom,
            };

            return _fixedCostRepository.AddFixedCostWithVersionAsync(
                newFixedCost, newFixedCostVersion, cancellationToken);
        }
    }

    private sealed class CommandValidator : BaseValidator<Command>
    {
        protected override void ValidateInternal(Command entity)
        {
            IsNotEmpty(entity.Name, "Name is required.");

            IsGreaterThan(entity.DayOfMonth, 0, "Day of month must be greater than 0.");
            IsLessThan(entity.DayOfMonth, 29, "Day of month must be less than 29.");

            IsPositiveMoneyAmount(entity.InitialAmount, "Initial amount must be a positive value.");
        }
    }
}
