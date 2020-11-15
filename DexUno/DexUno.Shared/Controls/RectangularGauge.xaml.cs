using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Dex.Uwp.Controls
{
    public sealed partial class RectangularGauge : UserControl
    {
        public RectangularGauge()
        {
            this.InitializeComponent();
            this.Loaded += RectangularGauge_Loaded;
            InitAnimations();
        }

        private void InitAnimations()
        {
#if WINDOWS_UWP
            //TODO: This is not working! Maybe animation are only triggered when you manually change the visual size property.
            var visual = ElementCompositionPreview.GetElementVisual(ValueRectangle);
            var compositor = visual.Compositor;

            var sizeAnimation = compositor.CreateVector2KeyFrameAnimation();
            sizeAnimation.Target = "Size";
            sizeAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            sizeAnimation.Duration = TimeSpan.FromMilliseconds(1500);

            var implicitAnimations = compositor.CreateImplicitAnimationCollection();
            implicitAnimations["Size"] = sizeAnimation;

            visual.ImplicitAnimations = implicitAnimations;
#endif
        }

        private void RectangularGauge_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            InitAnimations();
        }
    }
}