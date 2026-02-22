using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Cards.Abstractions;
using HomeNet.Core.Modules.Cards.Models;

namespace HomeNet.Core.Modules.Cards.Commands;

public static class AddCard
{
    public sealed record Command : ICommand, IValidatable<Command>
    {
        public required string Name { get; init; }

        public required DateOnly ExpirationDate { get; init; }

        public required int PersonId { get; init; }

        public ValidationResult Validate()
            => new CommandValidator().Validate(this);
    }

    public sealed class CommandHandler : ICommandHandler<Command>
    {
        private readonly ICardRepository _cardRepository;

        public CommandHandler(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public Task<Result> HandleAsync(
            Command command, 
            CancellationToken cancellationToken = default)
        {
            var validationResult = command.Validate();

            if (!validationResult.IsValid)
            {
                return validationResult.ToFailure();
            }

            var newCard = new Card
            {
                Name = command.Name,
                ExpirationDate = command.ExpirationDate,
                PersonId = command.PersonId,
            };

            return _cardRepository.AddCardAsync(newCard, cancellationToken);
        }
    }

    private sealed class CommandValidator : BaseValidator<Command>
    {
        protected override void ValidateInternal(Command entity)
        {
            IsNotEmpty(entity.Name, "Name is required");

            if (entity.ExpirationDate <= DateOnly.FromDateTime(DateTime.UtcNow))
            {
                Errors.Add("Expiration date must be in the future.");
            }
        }
    }
}
