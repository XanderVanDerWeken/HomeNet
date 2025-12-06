namespace HomeNet.Core.Modules.Cards.Models;

public sealed class Card
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required DateTimeOffset ExpirationDate { get; set; }
}
