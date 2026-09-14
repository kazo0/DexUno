namespace Dexter.Presentation;

/// <summary>Everything the Pokémon detail view shows for one Pokémon.</summary>
public sealed record PokemonDetails(
    Pokemon Pokemon,
    StatGauge Attack,
    StatGauge Defense,
    StatGauge Stamina,
    PokemonMoves MoveSet,
    IReadOnlyList<EvolutionLineItem> EvolutionLines,
    bool HasPrevious,
    bool HasNext)
{
    public bool HasEvolutions => EvolutionLines.Count > 0;
}

public sealed record EvolutionLineItem(IReadOnlyList<EvolutionStage> Stages);

/// <param name="IsFirst">False when an arrow should precede the stage.</param>
/// <param name="IsCurrent">True for the Pokémon currently displayed.</param>
public sealed record EvolutionStage(Pokemon Pokemon, bool IsFirst, bool IsCurrent);
