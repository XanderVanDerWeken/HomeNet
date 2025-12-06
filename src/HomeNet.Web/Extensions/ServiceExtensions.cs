using HomeNet.Core.Common.Cqrs;
using HomeNet.Infrastructure.Events;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace HomeNet.Web.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddCommonServices(
        this IServiceCollection services)
    {
        services.AddSingleton<IMediator, EventBus>();

        return services;
    }

    public static IServiceCollection AddDatabase(
        this IServiceCollection services, 
        IConfiguration config)
    {
        services.AddSingleton<QueryFactory>(sp =>
        {
            var connectionString = config.GetConnectionString("Default");

            var connection = new NpgsqlConnection(connectionString);
            var compiler = new PostgresCompiler();

            return new QueryFactory(connection, compiler);
        });

        return services;
    }
}
