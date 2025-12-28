using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Cards.Abstractions;
using HomeNet.Core.Modules.Cards.Models;

namespace HomeNet.Core.Modules.Cards.Queries;

public sealed class CardsExpiringBeforeQueryHandler : IQueryHandler<CardsExpiringBeforeQuery, IReadOnlyList<Card>>
{
    private readonly ICardRepository _cardRepository;

    public CardsExpiringBeforeQueryHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<Result<IReadOnlyList<Card>>> HandleAsync(
        CardsExpiringBeforeQuery query, 
        CancellationToken cancellationToken = default)
    {
        var cards = await _cardRepository.GetAllCardsWithExpiryBeforeAsync(
            query.ExpiryDate, 
            cancellationToken);

        return Result<IReadOnlyList<Card>>.Success(cards);
    }
}
