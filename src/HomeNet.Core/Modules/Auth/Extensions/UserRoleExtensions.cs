using HomeNet.Core.Modules.Auth.Models;

namespace HomeNet.Core.Modules.Auth.Extensions;

public static class UserRoleExtensions
{
    public static string ToRoleString(this UserRole role) => role switch
    {
        UserRole.User => "User",
        UserRole.Admin => "Admin",
        _ => throw new ArgumentOutOfRangeException(nameof(role)),
    };

    public static UserRole ToRoleEnum(this string roleString) => roleString switch
    {
        "User" => UserRole.User,
        "Admin" => UserRole.Admin,
        _ => throw new ArgumentOutOfRangeException(nameof(roleString)),
    };
}
