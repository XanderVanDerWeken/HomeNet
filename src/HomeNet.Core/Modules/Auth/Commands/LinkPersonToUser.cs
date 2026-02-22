using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Errors;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Auth.Abstractions;
using HomeNet.Core.Modules.Persons.Abstractions;

namespace HomeNet.Core.Modules.Auth.Commands;

public static class LinkPersonToUser
{
    public sealed record Command : ICommand, IValidatable<Command>
    {
        public required string UserName { get; init; }

        public required int PersonId { get; init; }

        public ValidationResult Validate()
            => new CommandValidator().Validate(this);
    }

    public sealed class CommandHandler : ICommandHandler<Command>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPersonRepository _personRepository;

        public CommandHandler(
            IUserRepository userRepository, 
            IPersonRepository personRepository)
        {
            _userRepository = userRepository;
            _personRepository = personRepository;
        }

        public async Task<Result> HandleAsync(
            Command command, CancellationToken cancellationToken = default)
        {
            var validationResult = command.Validate();

            if (!validationResult.IsValid)
            {
                return validationResult.ToFailure();
            }

            var userToLink = await _userRepository.GetUserByUsernameAsync(
                command.UserName);
            
            if (userToLink is null)
            {
                return new NotFoundError("User", command.UserName).ToFailure();
            }

            var personToLink = await _personRepository.GetPersonByIdAsync(
                command.PersonId, cancellationToken);
            
            if (personToLink is null)
            {
                return new NotFoundError("Person", command.PersonId).ToFailure();
            }

            return await _userRepository.UpdatePersonLinkAsync(
                userToLink.Id,
                personToLink.Id,
                cancellationToken);
        }
    }

    public sealed class CommandValidator : BaseValidator<Command>
    {
        protected override void ValidateInternal(Command entity)
        {
            IsNotEmpty(entity.UserName, "Username cannot be empty.");

            if (entity.PersonId <= 0)
            {
                Errors.Add("PersonId must be greater than zero.");
            }
        }
    }
}
