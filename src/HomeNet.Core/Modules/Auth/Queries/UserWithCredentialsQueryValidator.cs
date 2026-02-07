using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Auth.Queries;

public sealed class UserWithCredentialsQueryValidator 
    : BaseValidator<UserWithCredentialsQuery>
{
    protected override void ValidateInternal(UserWithCredentialsQuery entity)
    {
        if (string.IsNullOrWhiteSpace(entity.UserName))
        {
            Errors.Add("UserName cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(entity.Password))
        {
            Errors.Add("Password cannot be empty.");
        }
    }
}
