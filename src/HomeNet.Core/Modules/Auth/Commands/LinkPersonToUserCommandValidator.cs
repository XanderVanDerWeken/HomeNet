using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Auth.Commands;

public sealed class LinkPersonToUserCommandValidator : BaseValidator<LinkPersonToUserCommand>
{
    protected override void ValidateInternal(LinkPersonToUserCommand entity)
    {
        IsNotEmpty(entity.UserName, "Username cannot be empty.");

        if (entity.PersonId <= 0)
        {
            Errors.Add("PersonId must be greater than zero.");
        }
    }
}
