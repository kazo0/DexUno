using Dex.Core.Entities;
using Dex.Uwp.Infrastructure;
using Dex.Uwp.ViewModels;

namespace Dex.Uwp.Pages
{
    public sealed partial class MovedexPage : PageBase
    {
        private MovedexViewModel vm;

        public MovedexPage()
        {
            this.InitializeComponent();
            QuickMovesList.ItemClick += MovesList_ItemClick;
            ChargeMovesList.ItemClick += MovesList_ItemClick;
            DataContextChanged += (a, b) =>
            {
                vm = b.NewValue as MovedexViewModel;
            };
        }

        private void MovesList_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            vm.OnMoveSelected((Move)e.ClickedItem);
        }
    }
}