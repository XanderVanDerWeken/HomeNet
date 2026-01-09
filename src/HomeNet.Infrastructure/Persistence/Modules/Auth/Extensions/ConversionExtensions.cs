using HomeNet.Core.Modules.Auth.Extensions;
using HomeNet.Core.Modules.Auth.Models;
using HomeNet.Infrastructure.Persistence.Modules.Auth.Entities;

namespace HomeNet.Infrastructure.Persistence.Modules.Auth.Extensions;

public static class ConversionExtensions
{
    public static User ToUser(this UserEntity entity)
        => new User
        {
            Id = entity.Id,
            Username = entity.Username,
            PasswordHash = entity.Password,
            Role = entity.Role.ToRoleEnum(),
        };
}
