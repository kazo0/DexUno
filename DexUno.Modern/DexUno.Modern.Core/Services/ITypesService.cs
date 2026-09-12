using DexUno.Modern.Core.Entities;

namespace DexUno.Modern.Core.Services;

public interface ITypesService
{
    Task<IReadOnlyList<TypeEffectiveness>> GetAllEffectivenessAsync(CancellationToken ct = default);

    IReadOnlyList<PokemonType> GetAllTypes();

    /// <summary>Combined damage multiplier of a move against a (possibly dual-typed) defender.</summary>
    Task<float> GetTypeAdvantageMultiplierAsync(Move attackingMove, Pokemon defendingPokemon, CancellationToken ct = default);
}
