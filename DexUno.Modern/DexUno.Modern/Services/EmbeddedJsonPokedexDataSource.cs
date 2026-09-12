using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace DexUno.Modern.Services;

/// <summary>Reads the pokedex JSON files embedded in the app assembly (see Data\*.json).</summary>
public sealed class EmbeddedJsonPokedexDataSource : IPokedexDataSource
{
    private const string ResourcePrefix = "Data.";

    public async Task<IReadOnlyList<Pokemon>> LoadPokemonsAsync(CancellationToken ct = default) =>
        await ReadAsync("pokemons.json", PokedexJsonContext.Default.ListPokemon, ct);

    public async Task<PokemonMoves> LoadMovesAsync(CancellationToken ct = default) =>
        await ReadAsync("moves.json", PokedexJsonContext.Default.PokemonMoves, ct);

    public async Task<IReadOnlyList<ushort[]>> LoadEvolutionLinesAsync(CancellationToken ct = default) =>
        await ReadAsync("evolutions.json", PokedexJsonContext.Default.ListUInt16Array, ct);

    public async Task<IReadOnlyList<TypeEffectiveness>> LoadTypeEffectivenessAsync(CancellationToken ct = default)
    {
        var table = await ReadAsync("types.json", PokedexJsonContext.Default.DictionaryStringTypeEffectivenessDto, ct);

        return table
            .Select(entry => new TypeEffectiveness(
                Enum.Parse<PokemonType>(entry.Key, ignoreCase: true),
                entry.Value.Strengths,
                entry.Value.Weaknesses))
            .ToArray();
    }

    private static async Task<T> ReadAsync<T>(string fileName, JsonTypeInfo<T> typeInfo, CancellationToken ct)
    {
        var assembly = typeof(EmbeddedJsonPokedexDataSource).Assembly;
        await using var stream = assembly.GetManifestResourceStream(ResourcePrefix + fileName)
            ?? throw new FileNotFoundException($"Embedded pokedex resource '{fileName}' was not found.");

        return await JsonSerializer.DeserializeAsync(stream, typeInfo, ct)
            ?? throw new InvalidDataException($"Embedded pokedex resource '{fileName}' is empty.");
    }
}

/// <summary>Shape of one entry in types.json (keyed by the attacking type).</summary>
public sealed record TypeEffectivenessDto(PokemonType[] Strengths, PokemonType[] Weaknesses);

[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    UseStringEnumConverter = true,
    GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(List<Pokemon>))]
[JsonSerializable(typeof(PokemonMoves))]
[JsonSerializable(typeof(List<ushort[]>))]
[JsonSerializable(typeof(Dictionary<string, TypeEffectivenessDto>))]
internal partial class PokedexJsonContext : JsonSerializerContext;
