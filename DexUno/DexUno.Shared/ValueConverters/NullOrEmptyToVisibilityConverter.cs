using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Dex.Uwp.ValueConverters
{
    public class NullOrEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object shouldReverse, string language)
        {
            var testObject = value as IEnumerable<object>;

            if (shouldReverse != null)
                return testObject == null || !testObject.Any() ? Visibility.Visible : Visibility.Collapsed;

            return testObject == null || !testObject.Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}