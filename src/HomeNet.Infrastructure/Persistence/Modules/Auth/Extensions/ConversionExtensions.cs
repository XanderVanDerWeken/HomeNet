using HomeNet.Core.Modules.Auth.Models;
using HomeNet.Infrastructure.Persistence.Modules.Auth.Entities;

namespace HomeNet.Infrastructure.Persistence.Modules.Auth.Extensions;

public static class ConversionExtensions
{
    public static User ToUser(this UserEntity entity)
        =>  new User
        {
            Id = entity.Id,
            UserName = entity.UserName,
            PasswordHash = entity.PasswordHash,
            Role = entity.Role,
        };
}
