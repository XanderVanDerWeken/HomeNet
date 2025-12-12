using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Cards.Abstractions;
using HomeNet.Core.Modules.Cards.Commands;
using HomeNet.Core.Modules.Cards.Queries;
using HomeNet.Infrastructure.Events;
using HomeNet.Infrastructure.Persistence.Modules.Cards;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;

namespace HomeNet.Web.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddCommonServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSingleton<IMediator, EventBus>();

        services.AddSingleton<QueryFactory>(sp =>
        {
            var connectionString = config.GetConnectionString("Default");

            var connection = new NpgsqlConnection(connectionString);
            var compiler = new PostgresCompiler();

            return new QueryFactory(connection, compiler);
        });

        return services;
    }

    public static IServiceCollection AddCardsModule(
        this IServiceCollection services)
    {
        services.AddScoped<ICardRepository, CardRepository>();

        services.AddScoped<AddCardCommandHandler>();
        services.AddScoped<RemoveCardCommandHandler>();

        services.AddScoped<CardsQueryHandler>();
        services.AddScoped<CardsExpiringBeforeQueryHandler>();

        return services;
    }
}
