using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Dex.Uwp.ValueConverters
{
    public class EvolutionLineToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var numberOfEvolutions = (int)value;
            var columnNumber = ushort.Parse((string)parameter);

            switch (columnNumber)
            {
                case 0:
                    return numberOfEvolutions >= 2 ? new GridLength(1, GridUnitType.Star) : new GridLength(0);

                case 1:
                    return numberOfEvolutions >= 2 ? new GridLength(16) : new GridLength(0);

                case 2:
                    return numberOfEvolutions >= 2 ? new GridLength(1, GridUnitType.Star) : new GridLength(0);

                case 3:
                    return numberOfEvolutions == 3 ? new GridLength(16) : new GridLength(0);

                case 4:
                    return numberOfEvolutions == 3 ? new GridLength(1, GridUnitType.Star) : new GridLength(0);

                default:
                    throw new Exception($"Invalid column number in {nameof(EvolutionLineToGridLengthConverter)}");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}