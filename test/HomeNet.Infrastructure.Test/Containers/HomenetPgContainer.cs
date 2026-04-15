using DotNet.Testcontainers.Builders;
using HomeNet.Infrastructure.Persistence.Modules.Auth;
using HomeNet.Infrastructure.Persistence.Modules.Cards;
using HomeNet.Infrastructure.Persistence.Modules.Persons;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace HomeNet.Infrastructure.Test.Containers;

public class HomenetPgContainer : IAsyncDisposable
{
    private PostgreSqlContainer _container;

    public HomenetPgContainer()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:18")
            .WithDatabase("homenet")
            .WithUsername("homenet_user")
            .WithPassword("homenet_password")
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

    public CardDbContext CreateCardDbContext()
        =>  new CardDbContext(new DbContextOptionsBuilder<CardDbContext>()
            .UseNpgsql(GetConnectionString()).Options);
    
    public UserDbContext CreateUserDbContext()
        => new UserDbContext(new DbContextOptionsBuilder<UserDbContext>()
            .UseNpgsql(GetConnectionString()).Options);
    
    public PersonDbContext CreatePersonDbContext()
        => new PersonDbContext(new DbContextOptionsBuilder<PersonDbContext>()
            .UseNpgsql(GetConnectionString()).Options);
}
