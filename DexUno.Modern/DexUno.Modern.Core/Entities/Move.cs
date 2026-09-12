using System.Diagnostics;
using System.Text.Json.Serialization;

namespace DexUno.Modern.Core.Entities;

[DebuggerDisplay("{Name} ({Type})")]
[JsonDerivedType(typeof(QuickMove))]
[JsonDerivedType(typeof(ChargeMove))]
public abstract class Move
{
    public required string MoveId { get; init; }

    public required string Name { get; init; }

    public PokemonType Type { get; init; }

    public ushort Damage { get; init; }

    /// <summary>Duration of the move in seconds.</summary>
    public float CoolDown { get; init; }

    [JsonIgnore]
    public abstract MoveType MoveType { get; }

    [JsonIgnore]
    public float DamagePerSecond => CoolDown <= 0 ? 0 : Damage / CoolDown;

    public override bool Equals(object? obj) => obj is Move other && other.MoveId == MoveId;

    public override int GetHashCode() => MoveId.GetHashCode();
}
