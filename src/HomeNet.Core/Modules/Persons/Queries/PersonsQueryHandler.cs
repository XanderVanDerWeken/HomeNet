using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Persons.Abstractions;
using HomeNet.Core.Modules.Persons.Models;

namespace HomeNet.Core.Modules.Persons.Queries;

public sealed class PersonsQueryHandler : IQueryHandler<PersonsQuery, IReadOnlyList<Person>>
{
    private readonly IPersonRepository _personRepository;

    public PersonsQueryHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<Result<IReadOnlyList<Person>>> HandleAsync(
        PersonsQuery query, 
        CancellationToken cancellationToken = default)
    {
        var persons = await _personRepository.GetAllPersonsAsync(
            query.includeInactivePersons,
            cancellationToken);

        return Result<IReadOnlyList<Person>>.Success(persons);
    }
}
