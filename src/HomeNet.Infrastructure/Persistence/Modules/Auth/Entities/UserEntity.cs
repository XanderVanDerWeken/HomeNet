using HomeNet.Core.Modules.Persons.Models;

namespace HomeNet.Infrastructure.Persistence.Modules.Auth.Entities;

public class UserEntity
{
    public int Id { get; set; }

    public required string UserName { get; set; }

    public required string PasswordHash { get; set; }

    public required string Role { get; set; }

    public Person? Person { get; set; }

    public int? PersonId { get; set; }
}
