using DexUno.Modern.Core.Entities;

namespace DexUno.Modern.Core.Repositories;

public interface IEvolutionsRepository
{
    /// <summary>Every evolution line the given Pokémon belongs to (some branch, e.g. Eevee).</summary>
    Task<IReadOnlyList<EvolutionLine>> GetEvolutionLinesAsync(Pokemon pokemon, CancellationToken ct = default);
}
