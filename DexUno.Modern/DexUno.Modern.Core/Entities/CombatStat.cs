namespace DexUno.Modern.Core.Entities;

/// <summary>A base combat stat (attack, defense or stamina) as stored in the pokedex data.</summary>
public sealed record CombatStat(ushort BaseValue) : IComparable<CombatStat>
{
    public ushort Min => (ushort)(BaseValue + IV.Min.Value);

    public ushort Max => (ushort)(BaseValue + IV.Max.Value);

    public ushort WithIv(IV iv) => (ushort)(BaseValue + iv.Value);

    public int CompareTo(CombatStat? other) => other is null ? 1 : BaseValue.CompareTo(other.BaseValue);
}
