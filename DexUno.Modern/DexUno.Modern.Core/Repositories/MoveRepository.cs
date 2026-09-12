using DexUno.Modern.Core.DataAccess;
using DexUno.Modern.Core.Entities;

namespace DexUno.Modern.Core.Repositories;

public sealed class MoveRepository : IMoveRepository
{
    private readonly IPokedexDataSource _dataSource;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private PokemonMoves? _cache;
    private Dictionary<string, Move>? _byId;

    public MoveRepository(IPokedexDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<PokemonMoves> GetAllAsync(CancellationToken ct = default)
    {
        if (_cache is { } cached)
        {
            return cached;
        }

        await _gate.WaitAsync(ct);
        try
        {
            if (_cache is null)
            {
                var loaded = await _dataSource.LoadMovesAsync(ct);
                _cache = new PokemonMoves(
                    loaded.QuickMoves.OrderBy(m => m.Name).ToArray(),
                    loaded.ChargeMoves.OrderBy(m => m.Name).ToArray());
                _byId = _cache.All.ToDictionary(m => m.MoveId, StringComparer.OrdinalIgnoreCase);
            }

            return _cache;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<Move?> GetByIdAsync(string moveId, CancellationToken ct = default)
    {
        await GetAllAsync(ct);
        return _byId!.GetValueOrDefault(moveId);
    }

    public async Task<PokemonMoves> GetForPokemonAsync(Pokemon pokemon, CancellationToken ct = default)
    {
        await GetAllAsync(ct);
        var index = _byId!;

        return new PokemonMoves(
            pokemon.Moves.QuickMovesIds.Select(id => index.GetValueOrDefault(id)).OfType<QuickMove>().ToArray(),
            pokemon.Moves.ChargeMovesIds.Select(id => index.GetValueOrDefault(id)).OfType<ChargeMove>().ToArray());
    }
}
