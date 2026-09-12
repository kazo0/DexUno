using Microsoft.UI.Xaml.Data;

namespace DexUno.Modern.Converters;

public sealed class UpperCaseConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, string language) =>
        value?.ToString()?.ToUpperInvariant();

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
