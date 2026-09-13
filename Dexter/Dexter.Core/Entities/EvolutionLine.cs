namespace Dexter.Core.Entities;

/// <summary>An ordered evolution chain, e.g. Bulbasaur → Ivysaur → Venusaur.</summary>
public sealed record EvolutionLine(IReadOnlyList<Pokemon> Stages)
{
    public int Count => Stages.Count;
}
