namespace DexUno.Modern.Presentation;

/// <summary>
/// Animated pokeball used as the app's loading indicator (FeedView progress template and splash screen).
/// Size it with Width/Height; it animates for as long as it is in the visual tree.
/// </summary>
public sealed partial class PokeballLoader : UserControl
{
    public PokeballLoader()
    {
        this.InitializeComponent();
        Loaded += (_, _) => WobbleStoryboard.Begin();
        Unloaded += (_, _) => WobbleStoryboard.Stop();
    }
}
