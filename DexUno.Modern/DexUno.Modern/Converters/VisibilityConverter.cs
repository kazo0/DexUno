using System.Collections;
using Microsoft.UI.Xaml.Data;

namespace DexUno.Modern.Converters;

/// <summary>
/// Collapses when the value is null, false, an empty string or an empty collection.
/// Pass "Invert" as the parameter to flip the result.
/// </summary>
public sealed class VisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, string language)
    {
        var hasValue = value switch
        {
            null => false,
            bool b => b,
            string s => s.Length > 0,
            IEnumerable e => e.Cast<object>().Any(),
            _ => true,
        };

        if (string.Equals(parameter as string, "Invert", StringComparison.OrdinalIgnoreCase))
        {
            hasValue = !hasValue;
        }

        return hasValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
