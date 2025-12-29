using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Persons.Commands;

public sealed record AddPersonCommand : ICommand, IValidatable<AddPersonCommand>
{
    public required string FirstName { get; init; }

    public required string LastName { get; init;}

    public string? AliasName { get; init; }

    public ValidationResult Validate()
        => new AddPersonCommandValidator().Validate(this);
}
