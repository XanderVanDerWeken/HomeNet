using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Auth.Commands;

public class UnlinkPersonFromUserCommandValidator : BaseValidator<UnlinkPersonFromUserCommand>
{
    protected override void ValidateInternal(UnlinkPersonFromUserCommand entity)
    {
        IsNotEmpty(entity.UserName, "UserName cannot be empty.");
    }
}
