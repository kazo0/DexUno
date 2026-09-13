namespace Dexter.Presentation;

public sealed partial class SearchPage : Page
{
    /// <summary>The results scroller, captured from inside the FeedView template so it can be reset.</summary>
    private ScrollViewer? _listScroll;

    public SearchPage()
    {
        this.InitializeComponent();
    }

    private void OnListScrollViewerLoaded(object sender, RoutedEventArgs e) => _listScroll = sender as ScrollViewer;

    /// <summary>
    /// A new query reloads the paginated results from their first page, but the scroller keeps the offset it
    /// was left at — which on a shorter result set leaves the user looking at blank space. Go back to the top.
    /// </summary>
    private void OnSearchTextChanged(object sender, object e) =>
        _listScroll?.ChangeView(null, 0, null, disableAnimation: true);
}
