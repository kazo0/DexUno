namespace DexUno.Modern.Core.Entities;

public sealed record PokemonMoves(IReadOnlyList<QuickMove> QuickMoves, IReadOnlyList<ChargeMove> ChargeMoves)
{
    public static readonly PokemonMoves Empty = new([], []);

    public IEnumerable<Move> All => QuickMoves.Cast<Move>().Concat(ChargeMoves);
}
