using Dex.Uwp.Services;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using DexUno.Shared;
using DexUno.Shared.Helpers;

namespace Dex.Uwp.Pages
{
    public sealed partial class SidePane : UserControl
    {
        private Dictionary<string, Action> _menuItemToPage;
        private INavigationService _navigationService;

        public SidePane()
        {
            InitializeComponent();
            InitMenuItemToPage();

            MyList.ItemsSource = _menuItemToPage.Keys;
            MyList.ItemClick += MyList_ItemClick;
            Loaded += SidePane_Loaded;
        }

        private void InitMenuItemToPage()
        {
            //TODO: Make these automaticly resolved to avoid adding new entries each time
            _menuItemToPage = new Dictionary<string, Action>()
            {
                ["Pokedex"] = () => _navigationService.NavigateToPokedexPage(),
                ["Movedex"] = () => _navigationService.NavigateToMovesPage(),
                ["Types"] = () => _navigationService.NavigateToTypesPage(),
                ["Settings"] = () => _navigationService.NavigateToSettingsPage(),
                ["About"] = () => _navigationService.NavigateToMoveAboutPage(),
            };
        }

        private void MyList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var currentPage = _navigationService.CurrentPage.Name.Replace("Page", string.Empty);
            if ((string)e.ClickedItem != currentPage)
                _menuItemToPage[(string)e.ClickedItem].Invoke();
        }

        private void SidePane_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _navigationService = Startup.ServiceProvider.GetService<INavigationService>();
        }
    }
}