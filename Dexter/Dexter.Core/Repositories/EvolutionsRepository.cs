using Dexter.Core.DataAccess;
using Dexter.Core.Entities;

namespace Dexter.Core.Repositories;

public sealed class EvolutionsRepository : IEvolutionsRepository
{
    private readonly IPokedexDataSource _dataSource;
    private readonly IPokemonRepository _pokemons;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IReadOnlyList<ushort[]>? _lines;

    public EvolutionsRepository(IPokedexDataSource dataSource, IPokemonRepository pokemons)
    {
        _dataSource = dataSource;
        _pokemons = pokemons;
    }

    public async Task<IReadOnlyList<EvolutionLine>> GetEvolutionLinesAsync(Pokemon pokemon, CancellationToken ct = default)
    {
        var lines = await GetRawLinesAsync(ct);
        var all = await _pokemons.GetAllAsync(ct);
        var byDex = all.ToDictionary(p => p.DexNumber);

        return lines
            .Where(line => line.Length > 1 && line.Contains(pokemon.DexNumber))
            .Select(line => new EvolutionLine(line.Select(dex => byDex.GetValueOrDefault(dex)).OfType<Pokemon>().ToArray()))
            .Where(line => line.Count > 1)
            .ToArray();
    }

    private async Task<IReadOnlyList<ushort[]>> GetRawLinesAsync(CancellationToken ct)
    {
        if (_lines is { } cached)
        {
            return cached;
        }

        await _gate.WaitAsync(ct);
        try
        {
            return _lines ??= await _dataSource.LoadEvolutionLinesAsync(ct);
        }
        finally
        {
            _gate.Release();
        }
    }
}
