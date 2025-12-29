using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Persons.Abstractions;

namespace HomeNet.Core.Modules.Persons.Commands;

public sealed class UpdatePersonCommandHandler : ICommandHandler<UpdatePersonCommand>
{
    private readonly IPersonRepository _personRepository;

    public UpdatePersonCommandHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<Result> HandleAsync(
        UpdatePersonCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = command.Validate();

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ErrorMessage!);
        }

        var person = await _personRepository.GetPersonByIdAsync(command.PersonId);

        if (person == null)
        {
            return Result.Failure($"Person with ID {command.PersonId} not found.");
        }

        if (command.UpdatedFirstName != null)
        {
            person.FirstName = command.UpdatedFirstName;
        }
        if (command.UpdatedLastName != null)
        {
            person.LastName = command.UpdatedLastName;
        }
        if (command.UpdatedAliasName != null)
        {
            person.AliasName = command.UpdatedAliasName;
        }
        if (command.UpdatedIsInactive != null)
        {
            person.IsInactive = command.UpdatedIsInactive.Value;
        }

        return await _personRepository.UpdatePersonAsync(person);
    }
}
