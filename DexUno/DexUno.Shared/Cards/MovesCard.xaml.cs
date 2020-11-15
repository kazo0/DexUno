using Windows.UI.Xaml.Controls;

namespace Dex.Uwp.Cards
{
    public sealed partial class MovesCard : UserControl
    {
        public MovesCard()
        {
            this.InitializeComponent();
        }

        public event ItemClickEventHandler ItemClicked
        {
            add
            {
                QuickMovesList.ItemClick += value;
                ChargeMovesList.ItemClick += value;
            }

            remove
            {
                QuickMovesList.ItemClick -= value;
                ChargeMovesList.ItemClick -= value;
            }
        }
    }
}