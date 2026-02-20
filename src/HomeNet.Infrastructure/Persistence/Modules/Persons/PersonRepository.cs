using HomeNet.Core.Common;
using HomeNet.Core.Common.Errors;
using HomeNet.Core.Modules.Persons.Abstractions;
using HomeNet.Core.Modules.Persons.Models;
using HomeNet.Infrastructure.Persistence.Abstractions;
using HomeNet.Infrastructure.Persistence.Modules.Persons.Entities;
using HomeNet.Infrastructure.Persistence.Modules.Persons.Extensions;
using SqlKata;

namespace HomeNet.Infrastructure.Persistence.Modules.Persons;

public sealed class PersonRepository : SqlKataRepository, IPersonRepository
{
    private static readonly string TableName = "persons.persons";

    public PersonRepository(PostgresQueryFactory db)
        : base(db)
    {
    }

    public async Task<Result> AddPersonAsync(
        Person person, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new Query(TableName).AsInsert(new
            {
                first_name = person.FirstName,
                last_name = person.LastName,
                alias_name = person.AliasName,
                is_inactive = person.IsInactive,
            });

            var newPersonId = await InsertAndReturnIdAsync(query);
            person.Id = newPersonId;

            return Result.Success();
        }
        catch (Exception ex)
        {
            return new DatabaseError(TableName, ex).ToFailure();
        }
    }

    public async Task<Person?> GetPersonByIdAsync(
        int personId, 
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName)
            .Where("id", personId);
        
        var entity = await FirstOrDefaultAsync<PersonEntity>(
            query, 
            cancellationToken);
        
        return entity?.ToPerson();
    }

    public async Task<IReadOnlyList<Person>> GetAllPersonsAsync(
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var query = new Query(TableName);

        if (!includeInactive)
        {
            query.Where("is_inactive", false);
        }

        var entities = await GetMultipleAsync<PersonEntity>(
            query, 
            cancellationToken);
        
        return entities
            .Select(entity => entity.ToPerson())
            .ToList();
    }

    public async Task<Result> UpdatePersonAsync(
        Person person, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new Query(TableName)
                .Where("id", person.Id)
                .AsUpdate(new
                {
                    first_name = person.FirstName,
                    last_name = person.LastName,
                    alias_name = person.AliasName,
                    is_inactive = person.IsInactive,
                });

            var affectedRows = await ExecuteAsync(query, cancellationToken);

            return affectedRows > 0
                ? Result.Success()
                : new NotFoundError("Person", person.Id).ToFailure();
        }
        catch (Exception ex)
        {
            return new DatabaseError(TableName, ex).ToFailure();
        }
    }
}
