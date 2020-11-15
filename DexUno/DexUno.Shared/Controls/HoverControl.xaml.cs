using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Dex.Uwp.Controls
{
    public sealed partial class HoverControl : ContentControl
    {
        public static DependencyProperty HoverBackgroundProperty
        = DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(HoverControl), new PropertyMetadata(new SolidColorBrush((Color)Application.Current.Resources["SystemChromeLowColor"])));

        private Brush nonHoverBrush;
        private Border RootBorder;

        public HoverControl()
        {
            this.InitializeComponent();
            this.PointerEntered += HoverBorder_PointerEntered;
            this.PointerExited += HoverBorder_PointerExited;
        }

        public Brush HoverBackground
        {
            get { return (Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RootBorder = (Border)GetTemplateChild("RootBorder");
            nonHoverBrush = RootBorder.Background;
        }

        private void HoverBorder_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            RootBorder.Background = HoverBackground;
        }

        private void HoverBorder_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            RootBorder.Background = nonHoverBrush;
        }
    }
}