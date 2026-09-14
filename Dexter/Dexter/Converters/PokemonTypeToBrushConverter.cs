using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace Dexter.Converters;

/// <summary>Maps a <see cref="PokemonType"/> to the "{Type}TypeBrush" resource declared in Styles/AppResources.xaml.</summary>
public sealed class PokemonTypeToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        var type = value is PokemonType t ? t : PokemonType.Unknown;
        var resources = Application.Current.Resources;

        if (resources.TryGetValue($"{type}TypeBrush", out var brush) && brush is Brush typed)
        {
            return typed;
        }

        return resources.TryGetValue("UnknownTypeBrush", out var fallback) ? fallback : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
