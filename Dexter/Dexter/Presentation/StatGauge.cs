namespace Dexter.Presentation;

/// <summary>A base stat compared against the highest value in the pokedex.</summary>
public sealed record StatGauge(string Label, ushort Value, ushort Max)
{
    public double Percent => Max == 0 ? 0 : Math.Round(100d * Value / Max);
}
