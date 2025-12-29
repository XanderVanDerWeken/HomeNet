using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Persons.Commands;

public sealed record UpdatePersonCommand : ICommand, IValidatable<UpdatePersonCommand>
{
    public int PersonId { get; init; }

    public string? UpdatedFirstName { get; init; }

    public string? UpdatedLastName { get; init; }

    public string? UpdatedAliasName { get; init; }

    public bool? UpdatedIsInactive { get; init; }

    public ValidationResult Validate()
        => new UpdatePersonCommandValidator().Validate(this);
}
