using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Dex.Uwp.Controls
{
    public sealed partial class Badge : ContentControl
    {
        public static DependencyProperty TextProperty
      = DependencyProperty.Register("Text", typeof(string), typeof(Badge), new PropertyMetadata(string.Empty));

        public Badge()
        {
            this.InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}