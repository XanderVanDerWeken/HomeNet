using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Auth.Commands;

public sealed record UnlinkPersonFromUserCommand : ICommand, IValidatable<UnlinkPersonFromUserCommand>
{
    public required string UserName { get; init; }

    public ValidationResult Validate()
        => new UnlinkPersonFromUserCommandValidator().Validate(this);
}
