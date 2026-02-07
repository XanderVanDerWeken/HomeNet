using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Auth.Commands;

public sealed record AddUserCommand : ICommand, IValidatable<AddUserCommand>
{
    public required string UserName { get; init; }

    public required string Password { get; init; }

    public string Role { get; init; } = "User";

    public ValidationResult Validate()
        => new AddUserCommandValidator().Validate(this);
}
