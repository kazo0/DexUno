using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace Dexter.Presentation;

public sealed partial class Shell : UserControl, IContentControlProvider
{
    public Shell()
    {
        this.InitializeComponent();
#if __ANDROID__
        Loaded += (_, _) =>
        {
            ApplicationView.GetForCurrentView().VisibleBoundsChanged += OnVisibleBoundsChanged;
            ApplySafeAreaFallback();
        };
        Unloaded += (_, _) => ApplicationView.GetForCurrentView().VisibleBoundsChanged -= OnVisibleBoundsChanged;
#endif
    }

    public ContentControl ContentControl => Splash;

#if __ANDROID__
    private void OnVisibleBoundsChanged(ApplicationView sender, object args) => ApplySafeAreaFallback();

    /// <summary>
    /// The toolkit's SafeArea on <c>Root</c> skips its update while the window bounds and <c>VisibleBounds</c> disagree
    /// on orientation. That is meant to ride out a rotation, but it is permanently true on a near-square display such as
    /// a fold's inner screen (852×883 window, 852×795 visible once the status and gesture bars are taken out), so there
    /// it never pads anything. Pad the root ourselves in exactly that case, using the same orientation rule; whenever the
    /// two agree, SafeArea owns the padding and this does nothing.
    /// </summary>
    private void ApplySafeAreaFallback()
    {
        if (XamlRoot is not { } root)
        {
            return;
        }

        var bounds = new Rect(0, 0, root.Size.Width, root.Size.Height);
        var visible = ApplicationView.GetForCurrentView().VisibleBounds;

        if (Orientation(bounds) == Orientation(visible))
        {
            return;
        }

        Root.Padding = new Thickness(
            Math.Max(0, visible.Left - bounds.Left),
            Math.Max(0, visible.Top - bounds.Top),
            Math.Max(0, bounds.Right - visible.Right),
            Math.Max(0, bounds.Bottom - visible.Bottom));

        static int Orientation(Rect rect) => rect.Height > rect.Width ? 1 : rect.Width > rect.Height ? 2 : 0;
    }
#endif
}
