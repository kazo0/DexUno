namespace DexUno.Modern.Core.Entities;

public sealed record TypeEffectiveness(PokemonType ConcernedType, IReadOnlyList<PokemonType> StrongAgainst, IReadOnlyList<PokemonType> WeakAgainst);
