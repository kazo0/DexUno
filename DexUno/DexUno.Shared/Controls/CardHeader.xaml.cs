using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Dex.Uwp.Controls
{
    public sealed partial class CardHeader : ContentControl
    {
        public static DependencyProperty TitleProperty
       = DependencyProperty.Register("Title", typeof(string), typeof(CardHeader), new PropertyMetadata(string.Empty));

        public CardHeader()
        {
            this.InitializeComponent();
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
    }
}