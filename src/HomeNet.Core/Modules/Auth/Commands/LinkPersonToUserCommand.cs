using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Auth.Commands;

public sealed record LinkPersonToUserCommand : ICommand, IValidatable<LinkPersonToUserCommand>
{
    public required string UserName { get; init; }

    public required int PersonId { get; init; }

    public ValidationResult Validate()
        => new LinkPersonToUserCommandValidator().Validate(this);
}
