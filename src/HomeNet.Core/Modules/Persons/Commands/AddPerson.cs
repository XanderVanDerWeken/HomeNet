using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Persons.Abstractions;
using HomeNet.Core.Modules.Persons.Models;

namespace HomeNet.Core.Modules.Persons.Commands;

public static class AddPerson
{
    public sealed record Command : ICommand, IValidatable<Command>
    {
        public required string FirstName { get; init; }

        public required string LastName { get; init;}

        public string? AliasName { get; init; }

        public ValidationResult Validate()
            => new CommandValidator().Validate(this);
    }

    public sealed class CommandHandler : ICommandHandler<Command>
    {
        private readonly IPersonRepository _personRepository;

        public CommandHandler(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public Task<Result> HandleAsync(
            Command command, CancellationToken cancellationToken = default)
        {
            var validationResult = command.Validate();

            if (!validationResult.IsValid)
            {
                return validationResult.ToFailure();
            }

            var newPerson = new Person
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                AliasName = command.AliasName,
            };
            
            return _personRepository.AddPersonAsync(
                newPerson, cancellationToken);
        }
    }

    private sealed class CommandValidator : BaseValidator<Command>
    {
        protected override void ValidateInternal(Command entity)
        {
            IsNotEmpty(entity.FirstName, "First Name is required");

            IsNotEmpty(entity.LastName, "Last Name is required");
        }
    }
}
