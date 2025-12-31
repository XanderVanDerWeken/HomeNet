using System.Data.SQLite;
using Dapper;
using HomeNet.Web.Configurations;
using Microsoft.Extensions.Options;

namespace HomeNet.Web.Database;

public sealed class SqliteCacheInitializer
{
    private readonly string _connectionString;
    private readonly string _schemaFilePath;

    public SqliteCacheInitializer(
        string connectionString,
        IOptions<CacheInitializerConfiguration> config)
    {
        _connectionString = connectionString;

        _schemaFilePath = Path.Combine(
            AppContext.BaseDirectory, 
            config.Value.SchemaFileName);
    }

    public void Initialize()
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        using var tx = connection.BeginTransaction();

        var schemaSql = File.ReadAllText(_schemaFilePath);

        connection.ExecuteAsync(schemaSql, transaction: tx);

        tx.Commit();
    }
}
