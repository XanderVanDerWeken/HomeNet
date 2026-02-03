namespace HomeNet.Core.Modules.Auth.Models;

public sealed class User
{
    public int Id { get; set; }

    public required string UserName { get; set; }

    public required string PasswordHash { get; set; }

    public string Role { get; set; } = "User";
}
