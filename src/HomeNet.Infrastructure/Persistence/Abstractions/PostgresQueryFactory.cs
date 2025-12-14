using System.Data;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace HomeNet.Infrastructure.Persistence.Abstractions;

public sealed class PostgresQueryFactory : QueryFactory, IPostgresDb
{
    public PostgresQueryFactory(
        IDbConnection connection,
        Compiler compiler)
        : base(connection, compiler)
    {
    }
}
