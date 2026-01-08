using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Auth.Commands;

public class AddUserCommandValidator : BaseValidator<AddUserCommand>
{
    protected override void ValidateInternal(AddUserCommand entity)
    {
        IsNotEmpty(entity.Username, "Username must not be empty.");
        IsNotEmpty(entity.PasswordHash, "Password hash must not be empty.");
    }
}
