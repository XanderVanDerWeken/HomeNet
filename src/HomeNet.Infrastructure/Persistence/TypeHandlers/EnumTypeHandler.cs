using System.Data;
using Dapper;

namespace HomeNet.Infrastructure.Persistence.TypeHandlers;

public class EnumTypeHandler<T> : SqlMapper.TypeHandler<T>
    where T : struct, Enum
{
    public override void SetValue(IDbDataParameter parameter, T value)
    {
        parameter.Value = value.ToString();
        parameter.DbType = DbType.String;
    }

    public override T Parse(object value)
    {
        return Enum.Parse<T>(value.ToString()!, ignoreCase: true);
    }
}
