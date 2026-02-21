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
            .AddTransient<AddCard.CommandHandler>()
            .AddTransient<RemoveCard.CommandHandler>();

        services
            .AddTransient<AllCards.QueryHandler>()
            .AddTransient<AllCardsExpiringBefore.QueryHandler>();

        return services;
    }

    public static ICqrsBuilder AddCardsModule(this ICqrsBuilder builder)
    {
        builder.AddCommand<AddCard.Command, AddCard.CommandHandler>();
        builder.AddCommand<RemoveCard.Command, RemoveCard.CommandHandler>();

        builder.AddQuery<AllCards.Query, AllCards.QueryHandler, IReadOnlyList<Card>>();
        builder.AddQuery<AllCardsExpiringBefore.Query, AllCardsExpiringBefore.QueryHandler, IReadOnlyList<Card>>();

        return builder;
    }
}
