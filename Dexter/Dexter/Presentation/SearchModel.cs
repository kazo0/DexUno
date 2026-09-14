namespace Dexter.Presentation;

public partial record SearchModel(IPokemonRepository Pokemons, IMoveRepository Moves, INavigator Navigator)
{
    /// <summary>Page size used for the first page, before the list has measured how many items it can show.</summary>
    private const uint DefaultPageSize = 25;

    /// <summary>Last matches, reused across the page requests of a single query.</summary>
    private (string Query, IImmutableList<object> Matches)? _lastQuery;

    public string Title => "Search";

    public IState<string> SearchText => State.Value(this, () => string.Empty);

    /// <summary>
    /// None while the query is blank or nothing matches, otherwise the matches — Pokémon first, then moves —
    /// loaded one page at a time as the user scrolls.
    /// </summary>
    public IListFeed<object> Results => SearchText
        .Where(text => !string.IsNullOrWhiteSpace(text))
        .SelectPaginatedAsync<string, object>(async (text, page, ct) =>
        {
            var matches = await MatchesAsync(text, ct);

            return matches
                .Skip((int)page.CurrentCount)
                .Take((int)Math.Max(1, page.DesiredSize ?? DefaultPageSize))
                .ToImmutableList();
        });

    public async ValueTask SelectResult(object item, CancellationToken ct)
    {
        switch (item)
        {
            case Pokemon pokemon:
                await Navigator.NavigateViewModelAsync<PokemonDetailModel>(this, data: pokemon, cancellation: ct);
                break;
            case Move move:
                await Navigator.NavigateViewModelAsync<MoveDetailModel>(this, data: new MoveSelection(move), cancellation: ct);
                break;
        }
    }

    /// <summary>Everything matching the query; cached so each page request is a slice, not a new search.</summary>
    private async ValueTask<IImmutableList<object>> MatchesAsync(string text, CancellationToken ct)
    {
        var query = text.Trim();

        if (_lastQuery is { } last && last.Query == query)
        {
            return last.Matches;
        }

        var pokemons = await Pokemons.GetAllAsync(ct);
        var moves = await Moves.GetAllAsync(ct);

        var items = ImmutableList.CreateBuilder<object>();
        items.AddRange(pokemons.Where(p =>
            p.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
            || p.DexNumber.ToString() == query));
        items.AddRange(moves.All.Where(m => m.Name.Contains(query, StringComparison.OrdinalIgnoreCase)));

        var matches = items.ToImmutable();
        _lastQuery = (query, matches);

        return matches;
    }
}
