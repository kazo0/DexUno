using DexUno.Modern.Core.Entities;

namespace DexUno.Modern.Core.Repositories;

public interface IMoveRepository
{
    Task<PokemonMoves> GetAllAsync(CancellationToken ct = default);

    Task<Move?> GetByIdAsync(string moveId, CancellationToken ct = default);

    Task<PokemonMoves> GetForPokemonAsync(Pokemon pokemon, CancellationToken ct = default);
}
