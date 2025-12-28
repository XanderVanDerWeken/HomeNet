using System.Data;
using Dapper;
using HomeNet.Infrastructure.Persistence.TypeHandlers;
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
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
    }
}
