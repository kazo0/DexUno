namespace DexUno.Modern.Services;

/// <summary>Resolves the bundled artwork for a Pokémon. Only the first generation ships with pictures.</summary>
public static class PokemonImages
{
    public const ushort LastDexNumberWithArtwork = 151;

    private static readonly Uri Unknown = new("ms-appx:///Assets/Pokemon/unknown.png");

    public static Uri GetUri(ushort dexNumber) =>
        dexNumber is >= 1 and <= LastDexNumberWithArtwork
            ? new Uri($"ms-appx:///Assets/Pokemon/{dexNumber}.png")
            : Unknown;
}
