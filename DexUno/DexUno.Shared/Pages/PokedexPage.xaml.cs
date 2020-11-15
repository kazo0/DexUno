using Dex.Core.Entities;
using Dex.Uwp.Infrastructure;
using Dex.Uwp.ViewModels;
using DexUno.Shared;
using DexUno.Shared.Helpers;
using Windows.UI.Xaml.Navigation;

namespace Dex.Uwp.Pages
{
    public sealed partial class PokedexPage : PageBase
    {
        private PokedexViewModel vm;

        public PokedexPage()
        {
            this.InitializeComponent();
            AllPokemonsByDexNumberList.ItemClick += PokemonsList_ItemClick;
            AllPokemonsByCpList.ItemClick += PokemonsList_ItemClick;
            AllPokemonsByNameList.ItemClick += PokemonsList_ItemClick;
            AllPokemonsByTypeList.ItemClick += PokemonsList_ItemClick;

            DataContextChanged += (a, b) =>
            {
                vm = b.NewValue as PokedexViewModel;
            };
        }

        private void PokemonsList_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            vm.OnSelectNewItem((Pokemon)e.ClickedItem);
        }
	}
}