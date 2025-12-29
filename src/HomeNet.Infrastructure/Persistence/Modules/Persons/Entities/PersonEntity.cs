namespace HomeNet.Infrastructure.Persistence.Modules.Persons.Entities;

public sealed class PersonEntity
{
    public int Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public string? AliasName { get; set; }

    public bool IsInactive { get; set; } = false;
}
