using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Cards.Models;

namespace HomeNet.Core.Modules.Cards.Queries;

public class AllCardsQueryHandler : IQueryHandler<AllCardsQuery, IEnumerable<Card>>
{
    public Task<Common.Result<IEnumerable<Card>>> HandleAsync(
        AllCardsQuery query, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
