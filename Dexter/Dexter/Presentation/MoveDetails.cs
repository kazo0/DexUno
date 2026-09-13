namespace Dexter.Presentation;

/// <summary>Everything the move detail view shows for one move, apart from the paginated "used by" list.</summary>
public sealed record MoveDetails(Move Move)
{
    public QuickMove? QuickMove => Move as QuickMove;

    public ChargeMove? ChargeMove => Move as ChargeMove;

    public string DamagePerSecond => Move.DamagePerSecond.ToString("0.0");
}
