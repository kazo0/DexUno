using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Dex.Uwp.ValueConverters
{
    public class PokemonUnknownTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((Core.Entities.PokemonType)value) == Core.Entities.PokemonType.Unknown ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}