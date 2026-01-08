using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;
using HomeNet.Core.Modules.Auth.Models;

namespace HomeNet.Core.Modules.Auth.Commands;

public sealed record AddUserCommand : ICommand, IValidatable<AddUserCommand>
{
    public required string Username { get; init; }

    public required string PasswordHash { get; init; }

    public required UserRole Role { get; init; }

    public ValidationResult Validate()
        => new AddUserCommandValidator().Validate(this);
}
