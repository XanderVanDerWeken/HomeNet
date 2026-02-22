using HomeNet.Core.Common;
using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Errors;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Persons.Abstractions;

namespace HomeNet.Core.Modules.Persons.Commands;

public static class UpdatePerson
{
    public sealed record Command : ICommand, IValidatable<Command>
    {
        public int PersonId { get; init; }

        public string? UpdatedFirstName { get; init; }

        public string? UpdatedLastName { get; init; }

        public string? UpdatedAliasName { get; init; }

        public bool? UpdatedIsInactive { get; init; }

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

        public async Task<Result> HandleAsync(
            Command command, CancellationToken cancellationToken = default)
        {
            var validationResult = command.Validate();

            if (!validationResult.IsValid)
            {
                return validationResult.ToFailure();
            }

            var person = await _personRepository.GetPersonByIdAsync(
                command.PersonId);

            if (person == null)
            {
                return new NotFoundError("Person", command.PersonId).ToFailure();
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

    private sealed class CommandValidator : BaseValidator<Command>
    {
        protected override void ValidateInternal(Command entity)
        {
            IsNullOrNotEmpty(entity.UpdatedFirstName, "Updated First Name cannot be empty");

            IsNullOrNotEmpty(entity.UpdatedLastName, "Updated Last Name cannot be empty");

            IsNullOrNotEmpty(entity.UpdatedAliasName, "Updated Alias Name cannot be empty");
        }

        private void IsNullOrNotEmpty(string? value, string errorMessage)
        {
            if (value != null)
            {
                IsNotEmpty(value, errorMessage);
            }
        }
    }
}
