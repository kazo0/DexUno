using Dex.Core.Entities;
using Dex.Uwp.Infrastructure;
using Dex.Uwp.ViewModels;

namespace Dex.Uwp.Pages
{
    public sealed partial class PokemonDetailPage : PageBase
    {
        private PokemonDetailViewModel vm;

        public PokemonDetailPage()
        {
            InitializeComponent();
            //DataContextChanged += (a, b) => vm = b.NewValue as PokemonDetailViewModel;
            MovesCard.ItemClicked += MovesCard_ItemClicked;
        }

        private void MovesCard_ItemClicked(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            vm.OnMoveSelected((Move)e.ClickedItem);
        }
    }
}