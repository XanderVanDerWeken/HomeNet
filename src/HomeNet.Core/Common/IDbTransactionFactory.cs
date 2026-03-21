using System.Data;

namespace HomeNet.Core.Common;

public interface IDbTransactionFactory
{
    public Task<IDbTransaction> BeginAsync(
        CancellationToken cancellationToken = default);
}
