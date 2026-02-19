using System.Data.Common;
using HomeNet.Core.Common.Events;
using HomeNet.Infrastructure.Events;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Web.Cqrs;
using Npgsql;
using SqlKata.Compilers;

namespace HomeNet.Web.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSingleton<PostgresQueryFactory>(sp =>
        {
            var dataSource = sp.GetRequiredService<NpgsqlDataSource>();

            var connection = new NpgsqlConnection(dataSource.ConnectionString);
            var compiler = new PostgresCompiler();

            return new PostgresQueryFactory(connection, compiler);
        });

        return services;
    }

    public static IServiceCollection AddCqrs(
        this IServiceCollection services,
        Action<ICqrsBuilder> configure)
    {
        var builder = new CqrsBuilder();

        configure(builder);

        services.AddSingleton<IEventBus>(sp =>
            new EventBus(
                sp.GetRequiredService<IServiceScopeFactory>(),
                builder.Commands,
                builder.Queries,
                builder.Events));

        return services;
    }
}
