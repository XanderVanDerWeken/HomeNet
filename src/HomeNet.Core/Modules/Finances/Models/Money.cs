namespace HomeNet.Core.Modules.Finances.Models;

public readonly struct Money : IEquatable<Money>, IComparable<Money>
{
    public decimal Amount { get; }

    public Money(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");

        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    public static Money Zero => new(0m);

    public static Money operator +(Money a, Money b)
        => new(a.Amount + b.Amount);
    
    public static Money operator -(Money a, Money b)
        => new(a.Amount - b.Amount);

    public override string ToString()
        => $"{Amount:0.00} €";

    #region Equality

    public bool Equals(Money other)
        => Amount == other.Amount;

    public override bool Equals(object? obj)
        => obj is Money other && Equals(other);

    public override int GetHashCode()
        => Amount.GetHashCode();
    
    public static bool operator ==(Money left, Money right)
        => left.Equals(right);

    public static bool operator !=(Money left, Money right)
        => !left.Equals(right);

    #endregion Equality

    #region Comparability

    public int CompareTo(Money other)
        => Amount.CompareTo(other.Amount);

    public static bool operator <(Money left, Money right)
        => left.CompareTo(right) < 0;

    public static bool operator >(Money left, Money right)
        => left.CompareTo(right) > 0;

    public static bool operator <=(Money left, Money right)
        => left.CompareTo(right) <= 0;

    public static bool operator >=(Money left, Money right)
        => left.CompareTo(right) >= 0;

    #endregion Comparability
}
