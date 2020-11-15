using Windows.UI;
using Windows.UI.Xaml;
using Dex.Uwp.Helpers;

namespace Dex.Uwp.Services
{
    public interface IApplicationWindowManager
    {
        void InitializeWindow();
    }

    public class ApplicationWindowManager : IApplicationWindowManager
    {
        public void InitializeWindow()
        {
            InitAppWindowColors();
        }

        public void SetAccentColor(Color accentColor)
        {
            var resources = Application.Current.Resources;
            resources["SystemAccentColor"] = accentColor;
            resources["SystemAccentColorDark1"] = accentColor.Darken(0.07f);
            resources["SystemAccentColorDark2"] = accentColor.Darken(0.14f);
            resources["SystemAccentColorDark3"] = accentColor.Darken(0.22f);
        }

        private void InitAppWindowColors()
        {
#if WINDOWS_UWP
            var resources = Application.Current.Resources;

            var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = (Color)resources["SystemAccentColorDark1"];

            titleBar.InactiveBackgroundColor = (Color)resources["SystemAccentColor"];
            titleBar.ButtonBackgroundColor = (Color)resources["SystemAccentColorDark1"];
            titleBar.ButtonHoverBackgroundColor = (Color)resources["SystemAccentColorDark2"];
            titleBar.ButtonPressedBackgroundColor = (Color)resources["SystemAccentColorDark3"];
            titleBar.ButtonInactiveBackgroundColor = (Color)resources["SystemAccentColor"];
#endif
        }
    }
}