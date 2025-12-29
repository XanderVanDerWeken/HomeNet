using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Persons.Abstractions;
using HomeNet.Core.Modules.Persons.Models;

namespace HomeNet.Core.Modules.Persons.Commands;

public sealed class AddPersonCommandHandler : ICommandHandler<AddPersonCommand>
{
    private readonly IPersonRepository _personRepository;

    public AddPersonCommandHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public Task<Result> HandleAsync(
        AddPersonCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = command.Validate();

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ErrorMessage!);
        }

        var newPerson = new Person
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            AliasName = command.AliasName,
        };
        
        return _personRepository.AddPersonAsync(newPerson, cancellationToken);
    }
}
