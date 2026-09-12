namespace DexUno.Modern.Core.Entities;

public sealed class ChargeMove : Move
{
    public float Critical { get; init; }

    public float Dodge { get; init; }

    public ushort EnergyBars { get; init; }

    public override MoveType MoveType => MoveType.Charge;
}
