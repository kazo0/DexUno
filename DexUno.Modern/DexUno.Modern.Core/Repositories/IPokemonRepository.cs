using DexUno.Modern.Core.Entities;

namespace DexUno.Modern.Core.Repositories;

public interface IPokemonRepository
{
    Task<IReadOnlyList<Pokemon>> GetAllAsync(CancellationToken ct = default);

    Task<Pokemon?> GetByDexNumberAsync(ushort dexNumber, CancellationToken ct = default);

    Task<Pokemon?> GetNextAsync(ushort dexNumber, CancellationToken ct = default);

    Task<Pokemon?> GetPreviousAsync(ushort dexNumber, CancellationToken ct = default);

    Task<IReadOnlyList<Pokemon>> GetWithMoveAsync(string moveId, CancellationToken ct = default);

    Task<StatMaximums> GetStatMaximumsAsync(CancellationToken ct = default);
}

/// <summary>Highest base stats across the whole pokedex, used to scale stat gauges.</summary>
public sealed record StatMaximums(ushort Attack, ushort Defense, ushort Stamina);
