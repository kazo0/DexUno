using Windows.Storage;
using Windows.UI;
using Dex.Uwp.Helpers;

namespace Dex.Uwp.Services
{
    public interface ISettingsService
    {
        Color AccentColor { get; set; }
    }

    public class SettingsService : ISettingsService
    {
        private const string AccentThemeColorDefaultValue = "#E53935";
        private const string AccentThemeColorSettingKey = "AccentColor";
        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public Color AccentColor
        {
            get { return ColorUtils.FromHex(_localSettings.Values[AccentThemeColorSettingKey] as string ?? AccentThemeColorDefaultValue); }
            set { _localSettings.Values[AccentThemeColorSettingKey] = value.ToHex(); }
        }
    }
}