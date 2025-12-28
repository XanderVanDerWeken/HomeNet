using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;

namespace HomeNet.Infrastructure.Test.Containers;

public class HomenetPgContainer : IAsyncDisposable
{
    private static readonly string initdbPath = Path.GetFullPath("postgres.initdb.sql");

    private PostgreSqlContainer _container;

    public HomenetPgContainer()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:18")
            .WithDatabase("homenet")
            .WithUsername("homenet_user")
            .WithPassword("homenet_password")
            .WithResourceMapping(initdbPath, "/docker-entrypoint-initdb.d")
            .WithCleanUp(true)
            .WithWaitStrategy(
                 Wait.ForUnixContainer()
                    .UntilMessageIsLogged("PostgreSQL init process complete; ready for start up")
                    .UntilMessageIsLogged("database system is ready to accept connections"))
            .Build();
    }

    public string GetConnectionString() => _container.GetConnectionString();

    public Task StartAsync() => _container.StartAsync();

    public Task StopAsync() => _container.StopAsync();

    public ValueTask DisposeAsync() => _container.DisposeAsync();
}
