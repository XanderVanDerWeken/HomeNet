using HomeNet.Core.Modules.Persons.Abstractions;
using HomeNet.Core.Modules.Persons.Commands;
using HomeNet.Core.Modules.Persons.Models;
using HomeNet.Core.Modules.Persons.Queries;
using HomeNet.Infrastructure.Persistence.Modules.Persons;
using HomeNet.Web.Cqrs;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HomeNet.Web.Extensions;

public static class PersonsModuleExtensions
{
    public static IServiceCollection AddPersonsModule(
        this IServiceCollection services)
    {
        services.TryAddScoped<IPersonRepository, PersonRepository>();

        services
            .AddTransient<AddPersonCommandHandler>()
            .AddTransient<UpdatePersonCommandHandler>();
        
        services
            .AddTransient<PersonsQueryHandler>();

        return services;
    }

    public static ICqrsBuilder AddPersonsModule(this ICqrsBuilder builder)
    {
        builder.AddCommand<AddPersonCommand, AddPersonCommandHandler>();
        builder.AddCommand<UpdatePersonCommand, UpdatePersonCommandHandler>();

        builder.AddQuery<PersonsQuery, PersonsQueryHandler, IReadOnlyList<Person>>();

        return builder;
    }
}
