using HomeNet.Core.Common;
using HomeNet.Core.Common.Errors;
using HomeNet.Core.Modules.Persons.Abstractions;
using HomeNet.Core.Modules.Persons.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeNet.Infrastructure.Persistence.Modules.Persons;

public sealed class PersonRepository : IPersonRepository
{
    private readonly PersonDbContext _dbContext;

    public PersonRepository(PersonDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Person>> AddPersonAsync(Person person, CancellationToken cancellationToken = default)
    {
        await _dbContext.Persons.AddAsync(person, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(person);
    }

    public async Task<IReadOnlyList<Person>> GetAllPersonsAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Persons
            .AsNoTracking()
            .Where(p => includeInactive || !p.IsInactive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Person?> GetPersonByIdAsync(int personId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Persons
            .FindAsync(personId, cancellationToken);
    }

    public async Task<Result<Person>> UpdatePersonAsync(Person person, CancellationToken cancellationToken = default)
    {
        var affectedRows = await _dbContext.Persons
            .Where(p => p.Id == person.Id)
            .ExecuteUpdateAsync(update => update
                .SetProperty(p => p.FirstName, person.FirstName)
                .SetProperty(p => p.LastName, person.LastName)
                .SetProperty(p => p.AliasName, person.AliasName)
                .SetProperty(p => p.IsInactive, person.IsInactive),
                cancellationToken);
        
        return affectedRows > 0
            ? Result.Success(person)
            : new NotFoundError(nameof(Person), person.Id).ToFailure<Person>();
    }
}
