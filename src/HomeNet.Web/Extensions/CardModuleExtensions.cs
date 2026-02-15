using HomeNet.Core.Modules.Cards.Abstractions;
using HomeNet.Core.Modules.Cards.Commands;
using HomeNet.Core.Modules.Cards.Models;
using HomeNet.Core.Modules.Cards.Queries;
using HomeNet.Infrastructure.Persistence.Modules.Cards;
using HomeNet.Web.Cqrs;

namespace HomeNet.Web.Extensions;

public static class CardModuleExtensions
{
    public static IServiceCollection AddCardsModule(
        this IServiceCollection services)
    {
        services.AddScoped<ICardRepository, CardRepository>();

        services
            .AddTransient<AddCardCommandHandler>()
            .AddTransient<RemoveCardCommandHandler>();

        services
            .AddTransient<CardsQueryHandler>()
            .AddTransient<CardsExpiringBeforeQueryHandler>();

        return services;
    }

    public static ICqrsBuilder AddCardsModule(this ICqrsBuilder builder)
    {
        builder.AddCommand<AddCardCommand, AddCardCommandHandler>();
        builder.AddCommand<RemoveCardCommand, RemoveCardCommandHandler>();

        builder.AddQuery<CardsQuery, CardsQueryHandler, IReadOnlyList<Card>>();
        builder.AddQuery<CardsExpiringBeforeQuery, CardsExpiringBeforeQueryHandler, IReadOnlyList<Card>>();

        return builder;
    }
}
