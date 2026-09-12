namespace DexUno.Modern.Presentation;

public sealed partial class MovedexPage : Page
{
    /// <summary>Page width from which the detail is shown beside the list instead of on its own page.</summary>
    private const double WideBreakpoint = 900;

    /// <summary>The list's scroller, captured from inside the FeedView template so it can be reset.</summary>
    private ScrollViewer? _listScroll;

    public MovedexPage()
    {
        this.InitializeComponent();
        SizeChanged += (_, e) => UpdateLayout(e.NewSize.Width);
        DataContextChanged += (_, _) => UpdateLayout(ActualWidth);
    }

    private void UpdateLayout(double width)
    {
        var isWide = width >= WideBreakpoint;
        VisualStateManager.GoToState(this, isWide ? "Wide" : "Narrow", useTransitions: false);

        if (DataContext is MovedexViewModel vm)
        {
            _ = vm.Model.SetWideLayout(isWide, CancellationToken.None);
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
