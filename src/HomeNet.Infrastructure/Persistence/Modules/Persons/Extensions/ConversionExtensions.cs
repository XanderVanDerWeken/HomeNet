using HomeNet.Core.Modules.Persons.Models;
using HomeNet.Infrastructure.Persistence.Modules.Persons.Entities;

namespace HomeNet.Infrastructure.Persistence.Modules.Persons.Extensions;

public static class ConversionExtensions
{
    public static Person ToPerson(this PersonEntity entity)
    {
        return new Person
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            AliasName = entity.AliasName,
            IsInactive = entity.IsInactive,
        };
    }
}