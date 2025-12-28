using HomeNet.Core.Modules.Cards.Models;
using HomeNet.Infrastructure.Persistence.Modules.Cards.Entities;

namespace HomeNet.Infrastructure.Persistence.Modules.Cards.Extensions;

public static class ConversionExtensions
{
    public static CardEntity ToEntity(this Card card)
        => new CardEntity
        {
            Id = card.Id,
            Name = card.Name,
            ExpirationDate = card.ExpirationDate,
        };

    public static Card ToCard(this CardEntity entity)
        => new Card
        {
            Id = entity.Id,
            Name = entity.Name,
            ExpirationDate = entity.ExpirationDate,
        };
}
