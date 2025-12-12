using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Cards.Abstractions;
using HomeNet.Core.Modules.Cards.Models;

namespace HomeNet.Core.Modules.Cards.Queries;

public sealed class CardsQueryHandler : IQueryHandler<CardsQuery, IReadOnlyList<Card>>
{
    private readonly ICardRepository _cardRepository;

    public CardsQueryHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<Result<IReadOnlyList<Card>>> HandleAsync(
        CardsQuery query, 
        CancellationToken cancellationToken = default)
    {
        var cards = await _cardRepository.GetAllCardsAsync(cancellationToken);

        return Result<IReadOnlyList<Card>>.Success(cards);
    }
}
