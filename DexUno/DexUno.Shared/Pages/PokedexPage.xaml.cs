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
            DataContextChanged += (a, b) =>
            {
                vm = b.NewValue as PokedexViewModel;
            };
        }
	}
}