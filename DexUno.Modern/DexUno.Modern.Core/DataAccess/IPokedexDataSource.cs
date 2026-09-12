using DexUno.Modern.Core.Entities;

namespace DexUno.Modern.Core.DataAccess;

/// <summary>Raw access to the bundled pokedex data. Implementations are platform specific.</summary>
public interface IPokedexDataSource
{
    Task<IReadOnlyList<Pokemon>> LoadPokemonsAsync(CancellationToken ct = default);

    Task<PokemonMoves> LoadMovesAsync(CancellationToken ct = default);

    /// <summary>Each entry is an ordered list of dex numbers forming one evolution line.</summary>
    Task<IReadOnlyList<ushort[]>> LoadEvolutionLinesAsync(CancellationToken ct = default);

    Task<IReadOnlyList<TypeEffectiveness>> LoadTypeEffectivenessAsync(CancellationToken ct = default);
}
