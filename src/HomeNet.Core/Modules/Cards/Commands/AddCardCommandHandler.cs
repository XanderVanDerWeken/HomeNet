using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Cards.Abstractions;
using HomeNet.Core.Modules.Cards.Models;

namespace HomeNet.Core.Modules.Cards.Commands;

public sealed class AddCardCommandHandler : ICommandHandler<AddCardCommand>
{
    private readonly ICardRepository _cardRepository;

    public AddCardCommandHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public Task<Result> HandleAsync(
        AddCardCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = command.Validate();

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ErrorMessage!);
        }

        var newCard = new Card
        {
            Name = command.Name,
            ExpirationDate = command.ExpirationDate,
        };

        return _cardRepository.AddCardAsync(newCard, cancellationToken);
    }
}
