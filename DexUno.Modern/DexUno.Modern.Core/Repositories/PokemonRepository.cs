using DexUno.Modern.Core.DataAccess;
using DexUno.Modern.Core.Entities;

namespace DexUno.Modern.Core.Repositories;

public sealed class PokemonRepository : IPokemonRepository
{
    private readonly IPokedexDataSource _dataSource;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IReadOnlyList<Pokemon>? _cache;
    private StatMaximums? _maximums;

    public PokemonRepository(IPokedexDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IReadOnlyList<Pokemon>> GetAllAsync(CancellationToken ct = default)
    {
        if (_cache is { } cached)
        {
            return cached;
        }

        await _gate.WaitAsync(ct);
        try
        {
            return _cache ??= (await _dataSource.LoadPokemonsAsync(ct)).OrderBy(p => p.DexNumber).ToArray();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<Pokemon?> GetByDexNumberAsync(ushort dexNumber, CancellationToken ct = default)
    {
        var all = await GetAllAsync(ct);
        return all.FirstOrDefault(p => p.DexNumber == dexNumber);
    }

    public async Task<Pokemon?> GetNextAsync(ushort dexNumber, CancellationToken ct = default)
    {
        var all = await GetAllAsync(ct);
        return all.FirstOrDefault(p => p.DexNumber > dexNumber);
    }

    public async Task<Pokemon?> GetPreviousAsync(ushort dexNumber, CancellationToken ct = default)
    {
        var all = await GetAllAsync(ct);
        return all.LastOrDefault(p => p.DexNumber < dexNumber);
    }

    public async Task<IReadOnlyList<Pokemon>> GetWithMoveAsync(string moveId, CancellationToken ct = default)
    {
        var all = await GetAllAsync(ct);
        return all.Where(p => p.Moves.Contains(moveId)).ToArray();
    }

    public async Task<StatMaximums> GetStatMaximumsAsync(CancellationToken ct = default)
    {
        if (_maximums is { } max)
        {
            return max;
        }

        var all = await GetAllAsync(ct);
        return _maximums ??= new StatMaximums(
            all.Max(p => p.Attack.BaseValue),
            all.Max(p => p.Defense.BaseValue),
            all.Max(p => p.Stamina.BaseValue));
    }
}
