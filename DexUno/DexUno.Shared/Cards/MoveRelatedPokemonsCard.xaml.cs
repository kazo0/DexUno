using Windows.UI.Xaml.Controls;

namespace Dex.Uwp.Cards
{
    public sealed partial class MoveRelatedPokemonsCard : UserControl
    {
        public MoveRelatedPokemonsCard()
        {
            this.InitializeComponent();
        }

        public event ItemClickEventHandler ItemClicked
        {
            add
            {
                UsedByList.ItemClick += value;
            }

            remove
            {
                UsedByList.ItemClick -= value;
            }
        }
    }
}