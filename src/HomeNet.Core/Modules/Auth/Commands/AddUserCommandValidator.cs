using HomeNet.Core.Common.Validation;

namespace HomeNet.Core.Modules.Auth.Commands;

public sealed class AddUserCommandValidator : BaseValidator<AddUserCommand>
{
    protected override void ValidateInternal(AddUserCommand entity)
    {
        IsNotEmpty(entity.UserName, "User name must not be empty.");
        
        IsNotEmpty(entity.Password, "Password must not be empty.");
        
        if (entity.Role != "User" && entity.Role != "Admin")
        {
            Errors.Add("Role must be either 'User' or 'Admin'.");
        }
    }
}
