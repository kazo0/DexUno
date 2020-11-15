using System;
using Windows.UI.Xaml.Data;

namespace Dex.Uwp.ValueConverters
{
    public class VerticalTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var stringToConvert = value.ToString();
            return string.Join(Environment.NewLine, stringToConvert.ToUpperInvariant().ToCharArray());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}