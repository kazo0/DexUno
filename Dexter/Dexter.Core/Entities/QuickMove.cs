namespace Dexter.Core.Entities;

public sealed class QuickMove : Move
{
    public ushort EnergyGenerated { get; init; }

    public override MoveType MoveType => MoveType.Quick;
}
