namespace DexUno.Modern.Core.Entities;

/// <summary>Combat power. The displayed value is floored and never lower than 10.</summary>
public readonly record struct CP(double PreciseValue) : IComparable<CP>
{
    public ushort Value => (ushort)Math.Max(10, Math.Floor(PreciseValue));

    public int CompareTo(CP other) => Value.CompareTo(other.Value);

    public override string ToString() => Value.ToString();
}
