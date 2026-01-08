namespace HomeNet.Core.Modules.Auth.Models;

public sealed class User
{
    public int Id { get; set; }

    public required string Username { get; set; }

    public required string PasswordHash { get; set; }

    public required UserRole Role { get; set; } = UserRole.User;
}
