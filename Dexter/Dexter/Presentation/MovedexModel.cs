namespace Dexter.Presentation;

public partial record MovedexModel(IMoveRepository Moves, IPokemonRepository Pokemons, INavigator Navigator)
{
    public const int QuickMovesTab = 0;
    public const int ChargeMovesTab = 1;

    /// <summary>Page size used for the first page, before the list has measured how many items it can show.</summary>
    private const uint DefaultPageSize = 25;

    /// <summary>Last filtered/sorted result, reused across the page requests of a single set of criteria.</summary>
    private (string Search, int Tab, bool Descending, IImmutableList<Move> Matches)? _lastQuery;

    public string Title => "Movedex";

    public IState<string> SearchText => State.Value(this, () => string.Empty);

    public IState<int> TabIndex => State.Value(this, () => QuickMovesTab);

    public IState<bool> IsDescending => State.Value(this, () => false);

    /// <summary>Set by the page: when its TwoPaneView shows both panes, selection fills the detail pane instead of navigating.</summary>
    public IState<bool> IsTwoPane => State.Value(this, () => false);

    /// <summary>The highlighted row, two-way bound to the list's `ItemsRepeaterExtensions.SelectedItem`.</summary>
    public IState<Move> Selected => State<Move>.Empty(this);

    /// <summary>
    /// The list, loaded one page at a time as the user scrolls, and restarted from the first page
    /// whenever the search text, tab or sort direction changes.
    /// </summary>
    public IListFeed<Move> Items => Feed
        .Combine(SearchText, TabIndex, IsDescending)
        .SelectPaginatedAsync<(string Search, int Tab, bool Descending), Move>(async (criteria, page, ct) =>
        {
            var matches = await MatchesAsync(criteria.Search, criteria.Tab, criteria.Descending, ct);

            return matches
                .Skip((int)page.CurrentCount)
                .Take((int)Math.Max(1, page.DesiredSize ?? DefaultPageSize))
                .ToImmutableList();
        });

    /// <summary>Detail shown beside the list on wide layouts; exposed to the view as <see cref="MovedexViewModel.Detail"/>.</summary>
    internal MoveDetailModel DetailModel { get; } = new(null, Pokemons, Navigator);

    public async ValueTask ToggleSortDirection(CancellationToken ct) =>
        await IsDescending.UpdateAsync(descending => !descending, ct);

    public async ValueTask SelectMove(Move move, CancellationToken ct)
    {
        await Selected.UpdateAsync(_ => move, ct);

        if (await IsTwoPane)
        {
            await DetailModel.Show(move, ct);
        }
        else
        {
            await Navigator.NavigateViewModelAsync<MoveDetailModel>(this, data: new MoveSelection(move), cancellation: ct);
        }
    }

    /// <summary>Called by the page when its TwoPaneView switches between one pane and two.</summary>
    internal async ValueTask SetTwoPane(bool isTwoPane, CancellationToken ct)
    {
        await IsTwoPane.SetAsync(isTwoPane, ct);

        if (isTwoPane && !DetailModel.HasCurrent)
        {
            // Don't enumerate the paginated feed here: it would start a second, independent pagination.
            var matches = await MatchesAsync(await SearchText, await TabIndex, await IsDescending, ct);
            if (matches is [var first, ..])
            {
                await Selected.UpdateAsync(_ => first, ct);
                await DetailModel.Show(first, ct);
            }
        }
    }

    /// <summary>
    /// Everything matching the current criteria, in display order. Pagination slices this, so the result is
    /// cached until the criteria change — otherwise every page request would re-filter and re-sort the moves.
    /// </summary>
    private async ValueTask<IImmutableList<Move>> MatchesAsync(string? searchText, int tabIndex, bool isDescending, CancellationToken ct)
    {
        var search = searchText?.Trim() ?? string.Empty;

        if (_lastQuery is { } last && last.Search == search && last.Tab == tabIndex && last.Descending == isDescending)
        {
            return last.Matches;
        }

        var all = await Moves.GetAllAsync(ct);
        IEnumerable<Move> query = tabIndex == ChargeMovesTab ? all.ChargeMoves : all.QuickMoves;

        if (search.Length > 0)
        {
            query = query.Where(m => m.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        query = query.OrderBy(m => m.Name);
        if (isDescending)
        {
            query = query.Reverse();
        }

        var matches = query.ToImmutableList();
        _lastQuery = (search, tabIndex, isDescending, matches);

        return matches;
    }
}
