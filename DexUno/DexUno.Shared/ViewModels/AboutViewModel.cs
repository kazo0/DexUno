using Dex.Uwp.Infrastructure;
using Dex.Uwp.Services;
using System;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.System;

namespace Dex.Uwp.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        private readonly PackageVersion _appVersion;

        public AboutViewModel()
        {
            _appVersion = Package.Current.Id.Version;
            SendTweetCommand = new RelayCommand(async () => await Launcher.LaunchUriAsync(new Uri("http://twitter.com/home?status=@disklosr @Dexr")));
        }

        public string AppVersion => string.Format("{0}.{1}.{2}.{3}", _appVersion.Major, _appVersion.Minor, _appVersion.Build, _appVersion.Revision);

        public bool CanSendFeedback => false;

        public ICommand RateThisAppCommand { get; }

        public ICommand SendFeedbackCommand { get; }

        public ICommand SendTweetCommand { get; }
    }
}