using Dex.Core.Entities;
using Dex.Uwp.Pages;
using Microsoft.Extensions.Logging;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Type = System.Type;

namespace Dex.Uwp.Services
{
    public interface INavigationService
    {
        Type CurrentPage { get; }

        void GoBack();

        void NavigateToMoveAboutPage();

        void NavigateToMoveDetailsPage(string moveId);

        void NavigateToMovesPage();

        void NavigateToPokedexPage();

        void NavigateToPokemonDetailsPage(ushort dexNumber);

        void NavigateToSearchPage();

        void NavigateToSettingsPage();

        void NavigateToTypesPage();

        void ResolveFromThenNavigate(object paramObject);
    }

    public class NavigationService : INavigationService
    {
        private readonly ILogger log;
        private Frame mainFrame;

        public NavigationService(ILogger log)
        {
            this.log = log;
            EnsureNavigationFrameIsAvailable();
            InitBackButton();

            mainFrame.NavigationFailed += MainFrame_NavigationFailed;
            mainFrame.Navigating += MainFrame_Navigating;
            mainFrame.Navigated += MainFrame_Navigated;
        }

        public Type CurrentPage => mainFrame.Content.GetType();

        public void GoBack()
        {
            mainFrame.GoBack();
        }

        public void NavigateToMoveAboutPage()
        {
            mainFrame.Navigate(typeof(AboutPage));
        }

        public void NavigateToMoveDetailsPage(string moveId)
        {
            mainFrame.Navigate(typeof(MoveDetailPage), moveId);
        }

        public void NavigateToMovesPage()
        {
            mainFrame.Navigate(typeof(MovedexPage));
        }

        public void NavigateToPokedexPage()
        {
            log.LogDebug("[NavigationService] Navigating to Pokedex page");
            mainFrame.Navigate(typeof(PokedexPage));
        }

        public void NavigateToPokemonDetailsPage(ushort dexNumber)
        {
            mainFrame.Navigate(typeof(PokemonDetailPage), dexNumber);
        }

        public void NavigateToSearchPage()
        {
            mainFrame.Navigate(typeof(SearchPage));
        }

        public void NavigateToSettingsPage()
        {
            mainFrame.Navigate(typeof(SettingsPage));
        }

        public void NavigateToTypesPage()
        {
            mainFrame.Navigate(typeof(TypeEffectivenessPage));
        }

        public void ResolveFromThenNavigate(object navigationParam)
        {
            if (navigationParam is Pokemon)
                NavigateToPokemonDetailsPage((navigationParam as Pokemon).DexNumber);

            if (navigationParam is Move)
                NavigateToMoveDetailsPage((navigationParam as Move).MoveId);
        }

        private void EnsureNavigationFrameIsAvailable()
        {
            var content = Window.Current.Content;

            // The default state is that we expect to have the
            // AppShell as a hosting view for content
            if (content is Shell)
            {
                log.LogDebug("[NavigationService] Content is Shell");
                var appShell = content as Shell;
                mainFrame = appShell.Frame;
            }

            // We can also have a simple frame when the user
            // chooses to use the share target contract to share
            // photos from the Windows photos app.
            else if (content is Frame)
            {
                var frameShell = content as Frame;
                mainFrame = frameShell;
            }
            else
            {
                throw new NavigationServiceException($"Could not find navigation frame within app window.");
            }
        }

        private void InitBackButton()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) =>
            {
                EnsureNavigationFrameIsAvailable();
                if (mainFrame.CanGoBack)
                {
                    e.Handled = true;
                    mainFrame.GoBack();
                }
            };
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
        }

        private void MainFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            log.LogInformation("Requested navigation to {PageType} with param {@Param}.", e.SourcePageType.Name, e.Parameter);
        }

        private void MainFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            log.LogError("Navigation to {Page} Failed! (Exception: {Type}, Message: {Message}).", e.SourcePageType, e.Exception.GetType().Name, e.Exception.Message);
        }

        private void Navigate(Type pageType, string parameter = null, bool serializeParam = true)
        {
            EnsureNavigationFrameIsAvailable();

            if (parameter == null)
                mainFrame.Navigate(pageType);
            else
            {
                if (serializeParam)
                {
                    //TODO: Serialize param
                }

                mainFrame.Navigate(pageType, parameter);
            }
        }
    }
}