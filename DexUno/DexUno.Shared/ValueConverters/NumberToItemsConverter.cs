using System;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace Dex.Uwp.ValueConverters
{
    public class NumberToItemsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var numberOfItems = (ushort)value;
            return Enumerable.Range(0, numberOfItems);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}