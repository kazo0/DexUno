namespace DexUno.Modern.Presentation;

public partial record PokedexModel(
    IPokemonRepository Pokemons,
    IMoveRepository Moves,
    IEvolutionsRepository Evolutions,
    INavigator Navigator)
{
    public const int SortByNumber = 0;
    public const int SortByName = 1;
    public const int SortByType = 2;
    public const int SortByCp = 3;

    /// <summary>Page size used for the first page, before the list has measured how many items it can show.</summary>
    private const uint DefaultPageSize = 25;

    /// <summary>Last filtered/sorted result, reused across the page requests of a single set of criteria.</summary>
    private (string Search, int Sort, bool Descending, IImmutableList<Pokemon> Matches)? _lastQuery;

    public string Title => "Pokédex";

    public IState<string> SearchText => State.Value(this, () => string.Empty);

    public IState<int> SortIndex => State.Value(this, () => SortByNumber);

    public IState<bool> IsDescending => State.Value(this, () => false);

    /// <summary>Set by the page: when its TwoPaneView shows both panes, selection fills the detail pane instead of navigating.</summary>
    public IState<bool> IsTwoPane => State.Value(this, () => false);

    /// <summary>The highlighted row, two-way bound to the list's `ItemsRepeaterExtensions.SelectedItem`.</summary>
    public IState<Pokemon> Selected => State<Pokemon>.Empty(this);

    /// <summary>
    /// The list, loaded one page at a time as the user scrolls (the <c>ListView</c> drives this through
    /// <c>ISupportIncrementalLoading</c>), and restarted from the first page whenever the search text,
    /// sort column or direction changes.
    /// </summary>
    public IListFeed<Pokemon> Items => Feed
        .Combine(SearchText, SortIndex, IsDescending)
        .SelectPaginatedAsync<(string Search, int Sort, bool Descending), Pokemon>(async (criteria, page, ct) =>
        {
            var matches = await MatchesAsync(criteria.Search, criteria.Sort, criteria.Descending, ct);

            return matches
                .Skip((int)page.CurrentCount)
                .Take((int)Math.Max(1, page.DesiredSize ?? DefaultPageSize))
                .ToImmutableList();
        });

    /// <summary>Detail shown beside the list on wide layouts; exposed to the view as <see cref="PokedexViewModel.Detail"/>.</summary>
    internal PokemonDetailModel DetailModel { get; } = new(null, Pokemons, Moves, Evolutions, Navigator);

    public async ValueTask ToggleSortDirection(CancellationToken ct) =>
        await IsDescending.UpdateAsync(descending => !descending, ct);

    public async ValueTask SelectPokemon(Pokemon pokemon, CancellationToken ct)
    {
        await Selected.UpdateAsync(_ => pokemon, ct);

        if (await IsTwoPane)
        {
            await DetailModel.Show(pokemon, ct);
        }
        else
        {
            await Navigator.NavigateViewModelAsync<PokemonDetailModel>(this, data: pokemon, cancellation: ct);
        }
    }

    /// <summary>Called by the page when its TwoPaneView switches between one pane and two.</summary>
    internal async ValueTask SetTwoPane(bool isTwoPane, CancellationToken ct)
    {
        await IsTwoPane.SetAsync(isTwoPane, ct);

        if (isTwoPane && !DetailModel.HasCurrent)
        {
            // Don't enumerate the paginated feed here: it would start a second, independent pagination.
            var matches = await MatchesAsync(await SearchText, await SortIndex, await IsDescending, ct);
            if (matches is [var first, ..])
            {
                await Selected.UpdateAsync(_ => first, ct);
                await DetailModel.Show(first, ct);
            }
        }
    }

    /// <summary>
    /// Everything matching the current criteria, in display order. Pagination slices this, so the result is
    /// cached until the criteria change — otherwise every page request would re-filter and re-sort the pokedex.
    /// </summary>
    private async ValueTask<IImmutableList<Pokemon>> MatchesAsync(string? searchText, int sortIndex, bool isDescending, CancellationToken ct)
    {
        var search = searchText?.Trim() ?? string.Empty;

        if (_lastQuery is { } last && last.Search == search && last.Sort == sortIndex && last.Descending == isDescending)
        {
            return last.Matches;
        }

        IEnumerable<Pokemon> query = await Pokemons.GetAllAsync(ct);

        if (search.Length > 0)
        {
            query = query.Where(p =>
                p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                || p.DexNumber.ToString().StartsWith(search, StringComparison.Ordinal));
        }

        query = sortIndex switch
        {
            SortByName => query.OrderBy(p => p.Name),
            SortByType => query.OrderBy(p => p.PrimaryType.ToString()).ThenBy(p => p.DexNumber),
            SortByCp => query.OrderByDescending(p => p.Cp.Max.Value).ThenBy(p => p.DexNumber),
            _ => query.OrderBy(p => p.DexNumber),
        };

        if (isDescending)
        {
            query = query.Reverse();
        }

        var matches = query.ToImmutableList();
        _lastQuery = (search, sortIndex, isDescending, matches);

        return matches;
    }
}
