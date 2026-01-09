using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Auth.Queries;

public sealed record UserWithUsernameQuery : IQuery, IValidatable<UserWithUsernameQuery>
{
    public required string Username { get; init; }

    public ValidationResult Validate()
        => new UserWithUsernameQueryValidator().Validate(this);
}
