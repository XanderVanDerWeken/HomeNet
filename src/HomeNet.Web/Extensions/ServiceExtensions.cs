using HomeNet.Core.Common.Events;
using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Auth.Commands;
using HomeNet.Core.Modules.Auth.Queries;
using HomeNet.Core.Modules.Cards.Abstractions;
using HomeNet.Core.Modules.Cards.Commands;
using HomeNet.Core.Modules.Cards.Queries;
using HomeNet.Core.Modules.Finances.Abstractions;
using HomeNet.Core.Modules.Finances.Commands;
using HomeNet.Core.Modules.Finances.Queries;
using HomeNet.Core.Modules.Finances.Services;
using HomeNet.Core.Modules.Persons.Abstractions;
using HomeNet.Core.Modules.Persons.Commands;
using HomeNet.Core.Modules.Persons.Queries;
using HomeNet.Infrastructure.Cache.Modules.Finances;
using HomeNet.Infrastructure.Events;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Auth;
using HomeNet.Infrastructure.Persistence.Modules.Cards;
using HomeNet.Infrastructure.Persistence.Modules.Finances;
using HomeNet.Infrastructure.Persistence.Modules.Persons;
using HomeNet.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using SqlKata.Compilers;

namespace HomeNet.Web.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddCommonServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSingleton<IEventBus, EventBus>();

        services.AddSingleton<PostgresQueryFactory>(sp =>
        {
            var connectionString = config.GetConnectionString("Default");

            var connection = new NpgsqlConnection(connectionString);
            var compiler = new PostgresCompiler();

            return new PostgresQueryFactory(connection, compiler);
        });

        return services;
    }

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

        // TODO: Register Handlers with EventBus

        return services;
    }

    public static IServiceCollection AddAuthModule(
        this IServiceCollection services)
    {
        services
            .AddScoped<IPasswordService, PasswordService>()
            .AddScoped<IUserRepository, UserRepository>();
        services.TryAddScoped<IPersonRepository, PersonRepository>();

        services
            .AddTransient<AddUserCommandHandler>()
            .AddTransient<LinkPersonToUserCommandHandler>()
            .AddTransient<UnlinkPersonFromUserCommandHandler>();
        services.TryAddScoped<IPersonRepository, PersonRepository>();

        services.AddTransient<UserWithCredentialsQueryHandler>();

        // TODO: Register Handlers with EventBus

        return services;
    }

    public static IServiceCollection AddFinancesModule(
        this IServiceCollection services)
    {
        services
            .AddScoped<ICategoryRepository, CategoryRepository>()
            .AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddSingleton<IMonthlyTimelineRepository, MonthlyTimelineCache>();

        services.AddScoped<ITimelineBuilder, TimelineBuilder>();

        services
            .AddTransient<AddCategoryCommandHandler>()
            .AddTransient<AddExpenseCommandHandler>()
            .AddTransient<AddIncomeCommandHandler>();
        
        services
            .AddTransient<CategoriesQueryHandler>()
            .AddTransient<ExpensesQueryHandler>()
            .AddTransient<IncomesQueryHandler>()
            .AddTransient<MonthlyTimelineQueryHandler>();

        // TODO: Register Handlers with EventBus

        return services;
    }

    public static IServiceCollection AddPersonsModule(
        this IServiceCollection services)
    {
        services.TryAddScoped<IPersonRepository, PersonRepository>();

        services
            .AddTransient<AddPersonCommandHandler>()
            .AddTransient<UpdatePersonCommandHandler>();
        
        services
            .AddTransient<PersonsQueryHandler>();

        // TODO: Register Handlers with EventBus

        return services;
    }
}
