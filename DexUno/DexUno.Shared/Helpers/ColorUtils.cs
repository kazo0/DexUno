using System;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Dex.Uwp.Helpers
{
    public static class ColorUtils
    {
        public static SolidColorBrush BrushFromHex(string hexColor)
        {
            return new SolidColorBrush(FromHex(hexColor));
        }

        public static Color Darken(this Color color, float percentage)
        {
            if (percentage > 1 || percentage < 0) throw new ArgumentOutOfRangeException(nameof(percentage));
            return ChangeColorBrightness(color, -percentage);
        }

        public static Color FromHex(string hexColor)
        {
            hexColor = hexColor.Replace("#", string.Empty);
            var r = (byte)Convert.ToUInt32(hexColor.Substring(0, 2), 16);
            var g = (byte)Convert.ToUInt32(hexColor.Substring(2, 2), 16);
            var b = (byte)Convert.ToUInt32(hexColor.Substring(4, 2), 16);

            return Color.FromArgb(0xFF, r, g, b);
        }

        public static Color Lighten(this Color color, float percentage)
        {
            if (percentage > 1 || percentage < 0) throw new ArgumentOutOfRangeException(nameof(percentage));
            return ChangeColorBrightness(color, percentage);
        }

        public static string ToHex(this Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        private static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = color.R;
            float green = color.G;
            float blue = color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }
    }
}