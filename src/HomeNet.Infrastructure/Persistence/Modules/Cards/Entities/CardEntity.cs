namespace HomeNet.Infrastructure.Persistence.Modules.Cards.Entities;

public sealed class CardEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required DateOnly ExpirationDate { get; set; }

    public int PersonId { get; set; }
}
