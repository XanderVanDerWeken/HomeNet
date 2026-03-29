using HomeNet.Core.Modules.Cards.Models;
using HomeNet.Infrastructure.Persistence.Modules.Cards.Entities;

namespace HomeNet.Infrastructure.Persistence.Modules.Cards;

public static class ConversionExtensions
{
    public static CardEntity ToEntity(this Card card)
    {
        return new CardEntity
        {
            Id = card.Id,
            Name = card.Name,
            ExpirationDate = card.ExpirationDate,
            PersonId = card.PersonId,
        };
    }
}
