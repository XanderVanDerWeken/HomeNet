using System.Data;
using HomeNet.Core.Common;

namespace HomeNet.Infrastructure.Persistence.Abstractions;

public sealed class PostgresTransactionFactory : IDbTransactionFactory
{
    private readonly IDbConnection _dbConnection;

    public PostgresTransactionFactory(PostgresQueryFactory db)
    {
        _dbConnection = db.Connection;
    }

    public async Task<IDbTransaction> BeginAsync(CancellationToken cancellationToken = default)
    {
        if (_dbConnection.State != ConnectionState.Open)
        {
            _dbConnection.Open();
        }

        return _dbConnection.BeginTransaction();
    }
}
