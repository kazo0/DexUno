using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Dexter.Converters;

public sealed class DexNumberToImageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        if (value is null)
        {
            return null;
        }

        var dexNumber = System.Convert.ToUInt16(value);
        return new BitmapImage(PokemonImages.GetUri(dexNumber));
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
