using System.Diagnostics;
using System.Text.Json.Serialization;

namespace DexUno.Modern.Core.Entities;

[DebuggerDisplay("#{DexNumber} {Name}")]
public sealed class Pokemon
{
    private CpCalculator? _cp;

    public required ushort DexNumber { get; init; }

    public required string Name { get; init; }

    public PokemonType[] Types { get; init; } = [];

    public required CombatStat Attack { get; init; }

    public required CombatStat Defense { get; init; }

    public required CombatStat Stamina { get; init; }

    public ushort CatchRate { get; init; }

    public ushort FleeRate { get; init; }

    public PokemonMovesIds Moves { get; init; } = PokemonMovesIds.Empty;

    public ushort CandiesToEvolve { get; init; }

    public ushort EggDistance { get; init; }

    [JsonIgnore]
    public CpCalculator Cp => _cp ??= new CpCalculator(Attack, Defense, Stamina);

    [JsonIgnore]
    public PokemonType PrimaryType => Types.Length > 0 ? Types[0] : PokemonType.Unknown;

    [JsonIgnore]
    public string DisplayName => Name.Length == 0 ? Name : char.ToUpperInvariant(Name[0]) + Name[1..];

    public override bool Equals(object? obj) => obj is Pokemon other && other.DexNumber == DexNumber;

    public override int GetHashCode() => DexNumber.GetHashCode();
}
