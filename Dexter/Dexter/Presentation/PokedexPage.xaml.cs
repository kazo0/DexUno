namespace Dexter.Presentation;

public sealed partial class PokedexPage : Page
{
    /// <summary>The list's scroller, captured from inside the FeedView template so it can be reset.</summary>
    private ScrollViewer? _listScroll;

    public PokedexPage()
    {
        this.InitializeComponent();
        DataContextChanged += (_, _) => ApplyMode(Panes.Mode);
        Loaded += (_, _) => FoldPosture.Changed += OnFoldPostureChanged;
        Unloaded += (_, _) => FoldPosture.Changed -= OnFoldPostureChanged;
    }

    /// <summary>A fold posture change doesn't resize the window, so the TwoPaneView has to be told to look again.</summary>
    private void OnFoldPostureChanged(object? sender, EventArgs e) => FoldPosture.Refresh(Panes);

    /// <summary>
    /// The TwoPaneView decides whether pane 2 is on screen (Wide, Tall, or spanned across a hinge); the model
    /// only needs to know whether a selection goes to that pane or navigates to the detail page.
    /// </summary>
    private void OnModeChanged(TwoPaneView sender, object args) => ApplyMode(sender.Mode);

    private void ApplyMode(TwoPaneViewMode mode)
    {
        // The pane divider sits on the side that touches the list: left of it when side by side, above it when stacked.
        DetailPane.BorderThickness = mode == TwoPaneViewMode.Tall ? new Thickness(0, 1, 0, 0) : new Thickness(1, 0, 0, 0);

        if (DataContext is PokedexViewModel vm)
        {
            _ = vm.Model.SetTwoPane(mode != TwoPaneViewMode.SinglePane, CancellationToken.None);
        }
    }

    private void OnListScrollViewerLoaded(object sender, RoutedEventArgs e) => _listScroll = sender as ScrollViewer;

    /// <summary>
    /// New criteria reload the paginated list from its first page, but the scroller keeps the offset it was
    /// left at — which on a shorter result set leaves the user looking at blank space. Go back to the top.
    /// </summary>
    private void OnSortOrFilterChanged(object sender, object e) =>
        _listScroll?.ChangeView(null, 0, null, disableAnimation: true);
}
