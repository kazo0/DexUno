namespace Dexter.Presentation;

/// <summary>
/// Detail of one move. Used as the page model (navigated to with a <see cref="MoveSelection"/>) and as the
/// child model of <see cref="MovedexModel"/> for the wide-layout detail pane (created without an initial move).
/// </summary>
public partial record MoveDetailModel(MoveSelection? Initial, IPokemonRepository Pokemons, INavigator Navigator)
{
    /// <summary>Page size used for the first page, before the list has measured how many items it can show.</summary>
    private const uint DefaultPageSize = 20;

    /// <summary>Users of the last requested move, reused across its page requests.</summary>
    private (string MoveId, IReadOnlyList<Pokemon> Users)? _lastUsers;

    /// <summary>The move being displayed. None until one is selected.</summary>
    public IState<Move> Current => Initial is { } initial
        ? State.Value(this, () => initial.Move)
        : State<Move>.Empty(this);

    public IFeed<string> Title => Current.Select(move => move.Name);

    public IFeed<MoveDetails> Details => Current.Select(move => new MoveDetails(move));

    /// <summary>
    /// The Pokémon that can learn the move — a hundred of them for the common moves — loaded one page
    /// at a time as the user scrolls the detail view, and restarted whenever the move changes.
    /// </summary>
    public IListFeed<Pokemon> UsedBy => Current
        .SelectPaginatedAsync<Move, Pokemon>(async (move, page, ct) =>
        {
            var users = await UsersAsync(move, ct);

            return users
                .Skip((int)page.CurrentCount)
                .Take((int)Math.Max(1, page.DesiredSize ?? DefaultPageSize))
                .ToImmutableList();
        });

    /// <summary>True once a move has been shown; lets the parent decide whether to auto-select one.</summary>
    internal bool HasCurrent { get; private set; } = Initial is not null;

    public async ValueTask SelectPokemon(Pokemon pokemon, CancellationToken ct) =>
        await Navigator.NavigateViewModelAsync<PokemonDetailModel>(this, data: pokemon, cancellation: ct);

    internal async ValueTask Show(Move move, CancellationToken ct)
    {
        HasCurrent = true;
        await Current.UpdateAsync(_ => move, ct);
    }

    private async ValueTask<IReadOnlyList<Pokemon>> UsersAsync(Move move, CancellationToken ct)
    {
        if (_lastUsers is { } last && last.MoveId == move.MoveId)
        {
            return last.Users;
        }

        var users = await Pokemons.GetWithMoveAsync(move.MoveId, ct);
        _lastUsers = (move.MoveId, users);

        return users;
    }
}
