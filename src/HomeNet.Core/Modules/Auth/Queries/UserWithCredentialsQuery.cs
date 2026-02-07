using HomeNet.Core.Common.Cqrs;
using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Auth.Queries;

public sealed record UserWithCredentialsQuery : IQuery, IValidatable<UserWithCredentialsQuery>
{
    public required string UserName { get; init; }

    public required string Password { get; init; }

    public ValidationResult Validate()
        => new UserWithCredentialsQueryValidator().Validate(this);
}
