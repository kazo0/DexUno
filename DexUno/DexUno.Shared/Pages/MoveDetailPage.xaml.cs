using Dex.Core.Entities;
using Dex.Uwp.Infrastructure;
using Dex.Uwp.ViewModels;

namespace Dex.Uwp.Pages
{
    public sealed partial class MoveDetailPage : PageBase
    {
        private MoveDetailViewModel vm;

        public MoveDetailPage()
        {
            InitializeComponent();
            DataContextChanged += (a, b) => vm = b.NewValue as MoveDetailViewModel;
            UsedByCard.ItemClicked += UsedByCard_ItemClicked;
        }

        private void UsedByCard_ItemClicked(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            vm.OnPokemonSelected((Pokemon)e.ClickedItem);
        }
    }
}