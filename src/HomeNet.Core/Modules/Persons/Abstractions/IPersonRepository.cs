using HomeNet.Core.Common;
using HomeNet.Core.Modules.Persons.Models;

namespace HomeNet.Core.Modules.Persons.Abstractions;

public interface IPersonRepository
{
    Task<Result> AddPersonAsync(
        Person person, 
        CancellationToken cancellationToken = default);
    
    Task<Person?> GetPersonByIdAsync(
        int personId, 
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Person>> GetAllPersonsAsync(
        CancellationToken cancellationToken = default);
}
