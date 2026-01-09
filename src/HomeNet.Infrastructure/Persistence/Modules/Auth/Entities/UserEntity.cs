namespace HomeNet.Infrastructure.Persistence.Modules.Auth.Entities;

public sealed class UserEntity
{
    public int Id { get; set; }

    public required string Username { get; set; }

    public required string Password { get; set; }

    public required string Role { get; set; }
}
