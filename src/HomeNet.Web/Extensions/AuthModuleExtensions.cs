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
            .AddTransient<AddUserCommandHandler>()
            .AddTransient<LinkPersonToUserCommandHandler>()
            .AddTransient<UnlinkPersonFromUserCommandHandler>();
        services.TryAddScoped<IPersonRepository, PersonRepository>();

        services.AddTransient<UserWithCredentialsQueryHandler>();

        return services;
    }

    public static ICqrsBuilder AddAuthModule(this ICqrsBuilder builder)
    {
        builder.AddCommand<AddUserCommand, AddUserCommandHandler>();
        builder.AddCommand<LinkPersonToUserCommand, LinkPersonToUserCommandHandler>();
        builder.AddCommand<UnlinkPersonFromUserCommand, UnlinkPersonFromUserCommandHandler>();

        builder.AddQuery<UserWithCredentialsQuery, UserWithCredentialsQueryHandler, bool>();

        return builder;
    }
}
