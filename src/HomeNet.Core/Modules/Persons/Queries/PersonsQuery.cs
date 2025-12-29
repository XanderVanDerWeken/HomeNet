using HomeNet.Core.Common.Cqrs;

namespace HomeNet.Core.Modules.Persons.Queries;

public sealed record PersonsQuery : IQuery
{
    public bool includeInactivePersons { get; set; } = false;
}
