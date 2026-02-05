using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Persons.Abstractions;

namespace HomeNet.Core.Modules.Auth.Commands;

public sealed class LinkPersonToUserCommandHandler : ICommandHandler<LinkPersonToUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPersonRepository _personRepository;

    public LinkPersonToUserCommandHandler(
        IUserRepository userRepository, 
        IPersonRepository personRepository)
    {
        _userRepository = userRepository;
        _personRepository = personRepository;
    }

    public async Task<Result> HandleAsync(
        LinkPersonToUserCommand command, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = command.Validate();

        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult.ErrorMessage!);
        }

        var userToLink = await _userRepository.GetUserByUsernameAsync(command.UserName);
        if (userToLink is null)
        {
            return Result.Failure("User not found.");
        }

        var personToLink = await _personRepository.GetPersonByIdAsync(command.PersonId, cancellationToken);
        if (personToLink is null)
        {
            return Result.Failure("Person not found.");
        }

        return await _userRepository.UpdatePersonLinkAsync(
            userToLink.Id,
            personToLink.Id,
            cancellationToken);
    }
}
