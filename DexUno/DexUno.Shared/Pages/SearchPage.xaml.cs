using Dex.Uwp.Infrastructure;
using Dex.Uwp.Services;
using Dex.Uwp.ViewModels;
using DexUno.Shared;
using DexUno.Shared.Helpers;
using Windows.UI.Xaml.Navigation;

namespace Dex.Uwp.Pages
{
    public sealed partial class SearchPage : PageBase
    {
        private SearchViewModel vm;

        public SearchPage()
        {
            InitializeComponent();
            Loaded += SearchPage_Loaded;
            KeyUp += SearchPage_KeyUp;
            ResultsList.ItemClick += ResultsList_ItemClick;
            DataContextChanged += (a, b) =>
            {
                vm = b.NewValue as SearchViewModel;
            };
        }

        private void ResultsList_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            vm.OnItemChosen(e.ClickedItem);
        }

        private void SearchPage_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                var navService = Startup.ServiceProvider.GetService<INavigationService>();
                navService.GoBack();
            }
        }

        private void SearchPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SearchBox.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            Loaded -= SearchPage_Loaded;
        }
    }
}