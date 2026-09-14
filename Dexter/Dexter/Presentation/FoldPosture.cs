namespace Dexter.Presentation;

/// <summary>
/// Raised (on the UI thread) when a foldable device changes posture — flat, half-opened, spanned — without the window
/// changing size. <see cref="TwoPaneView"/> only re-evaluates its mode on size or XamlRoot changes, so pages that own one
/// call <see cref="Refresh"/> from this event. Fired by the Android head (see <c>HingeAwareSpanningRects</c>); other
/// platforms never raise it.
/// </summary>
internal static class FoldPosture
{
    public static event EventHandler? Changed;

    internal static void NotifyChanged() => Changed?.Invoke(null, EventArgs.Empty);

    /// <summary>
    /// Makes a TwoPaneView recompute its mode. <c>UpdateMode</c> is private, but changing any pane or threshold property
    /// calls it, so the tall threshold is nudged up and straight back.
    /// </summary>
    internal static void Refresh(TwoPaneView panes)
    {
        panes.MinTallModeHeight += 1;
        panes.MinTallModeHeight -= 1;
    }
}
