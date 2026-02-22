using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Persons.Abstractions;
using HomeNet.Core.Modules.Persons.Models;

namespace HomeNet.Core.Modules.Persons.Queries;

public static class AllPersons
{
    public sealed record Query : IQuery
    {
        public bool IncludeInactivePersons { get; init; } = false;
    }

    public sealed class QueryHandler : IQueryHandler<Query, IReadOnlyList<Person>>
    {
        private readonly IPersonRepository _personRepository;

        public QueryHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<Result<IReadOnlyList<Person>>> HandleAsync(
            Query query, CancellationToken cancellationToken = default)
        {
            var persons = await _personRepository.GetAllPersonsAsync(
                query.IncludeInactivePersons,
                cancellationToken);

            return Result<IReadOnlyList<Person>>.Success(persons);
        }
    }
}
