namespace HomeNet.Infrastructure.Persistence.Modules.Auth.Entities;

public sealed class UserEntity
{
    public int Id { get; set; }

    public required string UserName { get; set; }

    public required string PasswordHash { get; set; }

    public required string Role { get; set; }

    public int? PersonId { get; set; }
}
