using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Auth.Queries;

public sealed class UserWithUsernameQueryValidator : BaseValidator<UserWithUsernameQuery>
{
    protected override void ValidateInternal(UserWithUsernameQuery entity)
    {
        IsNotEmpty(entity.Username, "Username cannot be empty.");
    }
}
