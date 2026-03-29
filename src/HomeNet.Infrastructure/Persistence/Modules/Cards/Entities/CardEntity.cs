using HomeNet.Core.Modules.Persons.Models;

namespace HomeNet.Infrastructure.Persistence.Modules.Cards.Entities;

public class CardEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required DateOnly ExpirationDate { get; set; }

    public Person Person { get; set; }
    public required int PersonId { get; set; }
}
