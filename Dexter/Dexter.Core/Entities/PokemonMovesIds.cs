namespace Dexter.Core.Entities;

public sealed record PokemonMovesIds(IReadOnlyList<string> QuickMovesIds, IReadOnlyList<string> ChargeMovesIds)
{
    public static readonly PokemonMovesIds Empty = new([], []);

    public bool Contains(string moveId) => QuickMovesIds.Contains(moveId) || ChargeMovesIds.Contains(moveId);
}
