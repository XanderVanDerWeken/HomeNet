using System;

namespace HomeNet.Core.Modules.Finances.Models;

public sealed class FixedCost
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public List<FixedCostInterval> Intervals { get; set; } = [];

    public FixedCostInterval? LastInterval => Intervals
        .FirstOrDefault(i => i.EndDate == null);
}
