using System;
using Windows.UI.Xaml.Data;

namespace Dex.Uwp.ValueConverters
{
    public class PokemonType2TextFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var type = (Dex.Core.Entities.PokemonType)value;
            if (type == Core.Entities.PokemonType.Unknown)
                return string.Empty;
            return $"- {type.ToString()}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}