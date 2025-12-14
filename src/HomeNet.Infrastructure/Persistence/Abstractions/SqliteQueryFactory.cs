using System.Data;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace HomeNet.Infrastructure.Persistence.Abstractions;

public sealed class SqliteQueryFactory : QueryFactory, ISqliteDb
{
    public SqliteQueryFactory(
        IDbConnection connection,
        Compiler compiler)
        : base(connection, compiler)
    {
    }
}
