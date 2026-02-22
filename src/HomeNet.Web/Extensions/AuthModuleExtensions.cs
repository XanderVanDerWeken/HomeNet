using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Auth.Commands;
using HomeNet.Core.Modules.Auth.Queries;
using HomeNet.Core.Modules.Persons.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Auth;
using HomeNet.Infrastructure.Persistence.Modules.Persons;
using HomeNet.Infrastructure.Security;
using HomeNet.Web.Cqrs;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HomeNet.Web.Extensions;

public static class AuthModuleExtensions
{
    public static IServiceCollection AddAuthModule(
        this IServiceCollection services)
    {
        services
            .AddScoped<IPasswordService, PasswordService>()
            .AddScoped<IUserRepository, UserRepository>();
        services.TryAddScoped<IPersonRepository, PersonRepository>();

        services
            .AddTransient<AddUser.CommandHandler>()
            .AddTransient<LinkPersonToUser.CommandHandler>()
            .AddTransient<UnlinkPersonFromUser.CommandHandler>();
        services.TryAddScoped<IPersonRepository, PersonRepository>();

        services.AddTransient<UserWithCredentials.QueryHandler>();

        return services;
    }

    public static ICqrsBuilder AddAuthModule(this ICqrsBuilder builder)
    {
        builder.AddCommand<AddUser.Command, AddUser.CommandHandler>();
        builder.AddCommand<LinkPersonToUser.Command, LinkPersonToUser.CommandHandler>();
        builder.AddCommand<UnlinkPersonFromUser.Command, UnlinkPersonFromUser.CommandHandler>();

        builder.AddQuery<UserWithCredentials.Query, UserWithCredentials.QueryHandler, bool>();

        return builder;
    }
}
