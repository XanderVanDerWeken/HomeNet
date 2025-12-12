using HomeNet.Core.Common.Cqrs;

namespace HomeNet.Core.Modules.Cards.Queries;

public sealed record CardsExpiringBeforeQuery : IQuery
{
    public DateTimeOffset ExpiryDate { get; init; }
}
