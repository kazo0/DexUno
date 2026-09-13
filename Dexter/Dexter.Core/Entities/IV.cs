namespace Dexter.Core.Entities;

/// <summary>An individual value (0..15) applied on top of a base combat stat.</summary>
public readonly record struct IV
{
    public const ushort MaxValue = 15;

    public static readonly IV Max = new(MaxValue);
    public static readonly IV Min = new(0);

    public IV(ushort value)
    {
        if (value > MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, $"IV must be between 0 and {MaxValue}.");
        }

        Value = value;
    }

    public ushort Value { get; }
}
