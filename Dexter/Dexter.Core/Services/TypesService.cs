using Dexter.Core.DataAccess;
using Dexter.Core.Entities;

namespace Dexter.Core.Services;

public sealed class TypesService : ITypesService
{
    public const float NeutralMultiplier = 1f;
    public const float StrongMultiplier = 1.25f;
    public const float WeakMultiplier = 0.8f;

    private readonly IPokedexDataSource _dataSource;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IReadOnlyList<TypeEffectiveness>? _table;

    public TypesService(IPokedexDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IReadOnlyList<TypeEffectiveness>> GetAllEffectivenessAsync(CancellationToken ct = default)
    {
        if (_table is { } cached)
        {
            return cached;
        }

        await _gate.WaitAsync(ct);
        try
        {
            return _table ??= (await _dataSource.LoadTypeEffectivenessAsync(ct)).OrderBy(t => t.ConcernedType.ToString()).ToArray();
        }
        finally
        {
            _gate.Release();
        }
    }

    public IReadOnlyList<PokemonType> GetAllTypes() =>
        Enum.GetValues<PokemonType>().Where(t => t != PokemonType.Unknown).ToArray();

    public async Task<float> GetTypeAdvantageMultiplierAsync(Move attackingMove, Pokemon defendingPokemon, CancellationToken ct = default)
    {
        var table = await GetAllEffectivenessAsync(ct);
        var attacker = table.FirstOrDefault(t => t.ConcernedType == attackingMove.Type);
        if (attacker is null)
        {
            return NeutralMultiplier;
        }

        var multiplier = NeutralMultiplier;
        foreach (var defendingType in defendingPokemon.Types)
        {
            if (attacker.StrongAgainst.Contains(defendingType))
            {
                multiplier *= StrongMultiplier;
            }
            else if (attacker.WeakAgainst.Contains(defendingType))
            {
                multiplier *= WeakMultiplier;
            }
        }

        return multiplier;
    }
}
