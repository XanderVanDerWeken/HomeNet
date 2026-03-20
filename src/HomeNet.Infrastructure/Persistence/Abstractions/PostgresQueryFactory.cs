using System.Data.SqlTypes;
using System.Data;
using Dapper;
using HomeNet.Infrastructure.Persistence.TypeHandlers;
using SqlKata.Compilers;
using SqlKata.Execution;
using HomeNet.Core.Modules.Finances.Enums;

namespace HomeNet.Infrastructure.Persistence.Abstractions;

public sealed class PostgresQueryFactory : QueryFactory, IPostgresDb
{
    public PostgresQueryFactory(
        IDbConnection connection,
        Compiler compiler)
        : base(connection, compiler)
    {
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new EnumTypeHandler<TransactionType>());
        SqlMapper.AddTypeHandler(new EnumTypeHandler<TransactionSource>());
    }
}
