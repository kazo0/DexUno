using System;
using Windows.UI.Xaml.Data;

namespace Dex.Uwp.ValueConverters
{
    public class StringAppenderFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return $"{((ushort)value)}{(string)parameter}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}