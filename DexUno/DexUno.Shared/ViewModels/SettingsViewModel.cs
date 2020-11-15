using System.Collections.Generic;
using System.Linq;
using Dex.Uwp.DataAccess;
using Dex.Uwp.Infrastructure;
using System.Threading.Tasks;
using Windows.UI;
using Dex.Uwp.Helpers;
using Dex.Uwp.Services;

namespace Dex.Uwp.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IPokePicturesSourceProvider _picturesSourceProvider;
        private readonly ISettingsService _settingsService;

        public SettingsViewModel(ISettingsService settingsService, IPokePicturesSourceProvider picturesSourceProvider)
        {
            _settingsService = settingsService;
            _picturesSourceProvider = picturesSourceProvider;
            Source = _picturesSourceProvider.Source;
            AvailableColors = new[] {
                    "#E53935", "#D81B60", "#8E24AA", "#5E35B1", "#3949AB",
                    "#1E88E5", "#039BE5", "#00897B", "#43A047", "#689F38",
                    "#F4511E", "#6D4C41", "#757575", "#546E7A"
                }.Select(ColorUtils.FromHex).ToList();
        }

        public IEnumerable<Color> AvailableColors { get; }

        public IPokePicturesSource Source { get; }

        public Color SelectedColor
        {
            get { return AvailableColors.First(color => color.ToHex() == _settingsService.AccentColor.ToHex()); }
            set { _settingsService.AccentColor = value; }
        }

        public IPokePicturesSource SelectedSource
        {
            get { return _picturesSourceProvider.Source; }
            set { OnSourceSelectionChanged(value); }
        }

        private Task OnDownloadNewImages()
        {
            return Task.CompletedTask;
        }

        private async Task OnSourceSelectionChanged(IPokePicturesSource picturesSource)
        {
            var needInit = picturesSource as INeedInitialisation;
            if (needInit != null)
            {
                if (await needInit.IsInitialized())
                {
                    IsBusy = true;
                    await needInit.Initialize();
                }
            }

            _picturesSourceProvider.Source = picturesSource;
            OnPropertyChanged(nameof(SelectedSource));
            IsBusy = false;
        }
    }
}