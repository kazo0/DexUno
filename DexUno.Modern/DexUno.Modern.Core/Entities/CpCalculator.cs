namespace DexUno.Modern.Core.Entities;

/// <summary>Computes combat power from base stats, IVs and trainer level.</summary>
public sealed class CpCalculator
{
    private readonly CombatStat _attack;
    private readonly CombatStat _defense;
    private readonly CombatStat _stamina;

    public CpCalculator(CombatStat attack, CombatStat defense, CombatStat stamina)
    {
        _attack = attack ?? throw new ArgumentNullException(nameof(attack));
        _defense = defense ?? throw new ArgumentNullException(nameof(defense));
        _stamina = stamina ?? throw new ArgumentNullException(nameof(stamina));
    }

    /// <summary>Perfect IVs at max level.</summary>
    public CP Max => MaxAtLevel(CpMultipliersMap.MaxLevel);

    /// <summary>Worst IVs at level 1.</summary>
    public CP Min => MinAtLevel(CpMultipliersMap.MinLevel);

    public CP AtLevel(float level, IV? iv = null)
    {
        var effectiveIv = iv ?? IV.Min;
        return Calculate(_attack.WithIv(effectiveIv), _defense.WithIv(effectiveIv), _stamina.WithIv(effectiveIv), level);
    }

    public CP MaxAtLevel(float level) => Calculate(_attack.Max, _defense.Max, _stamina.Max, level);

    public CP MinAtLevel(float level) => Calculate(_attack.Min, _defense.Min, _stamina.Min, level);

    private static CP Calculate(ushort attack, ushort defense, ushort stamina, float level)
    {
        if (!CpMultipliersMap.LevelToCpm.TryGetValue(level, out var cpm))
        {
            throw new ArgumentException($"Invalid Pokémon level ({level}).", nameof(level));
        }

        var cp = attack * Math.Sqrt(defense) * Math.Sqrt(stamina) * Math.Pow(cpm, 2) / 10;
        return new CP(cp);
    }
}
