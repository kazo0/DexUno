namespace DexUno.Modern.Presentation;

/// <summary>
/// Detail of one Pokémon. Used as the page model (navigated to with a <see cref="Pokemon"/>) and as the
/// child model of <see cref="PokedexModel"/> for the wide-layout detail pane (created without an initial Pokémon).
/// </summary>
public partial record PokemonDetailModel(
    Pokemon? Initial,
    IPokemonRepository Pokemons,
    IMoveRepository Moves,
    IEvolutionsRepository Evolutions,
    INavigator Navigator)
{
    /// <summary>The Pokémon being displayed. None until one is selected.</summary>
    public IState<Pokemon> Current => Initial is { } initial
        ? State.Value(this, () => initial)
        : State<Pokemon>.Empty(this);

    public IFeed<string> Title => Current.Select(pokemon => pokemon.DisplayName);

    public IFeed<PokemonDetails> Details => Current.SelectAsync(async (pokemon, ct) => await LoadAsync(pokemon, ct));

    /// <summary>True once a Pokémon has been shown; lets the parent decide whether to auto-select one.</summary>
    internal bool HasCurrent { get; private set; } = Initial is not null;

    public async ValueTask Previous(CancellationToken ct)
    {
        if (await Current is { } current && await Pokemons.GetPreviousAsync(current.DexNumber, ct) is { } previous)
        {
            await Show(previous, ct);
        }
    }

    public async ValueTask Next(CancellationToken ct)
    {
        if (await Current is { } current && await Pokemons.GetNextAsync(current.DexNumber, ct) is { } next)
        {
            await Show(next, ct);
        }
    }

    public async ValueTask SelectMove(Move move, CancellationToken ct) =>
        await Navigator.NavigateViewModelAsync<MoveDetailModel>(this, data: new MoveSelection(move), cancellation: ct);

    public async ValueTask SelectEvolution(Pokemon pokemon, CancellationToken ct)
    {
        if ((await Current)?.DexNumber != pokemon.DexNumber)
        {
            await Show(pokemon, ct);
        }
    }

    internal async ValueTask Show(Pokemon pokemon, CancellationToken ct)
    {
        HasCurrent = true;
        await Current.UpdateAsync(_ => pokemon, ct);
    }

    private async Task<PokemonDetails> LoadAsync(Pokemon pokemon, CancellationToken ct)
    {
        var maximums = await Pokemons.GetStatMaximumsAsync(ct);
        var moveSet = await Moves.GetForPokemonAsync(pokemon, ct);
        var lines = await Evolutions.GetEvolutionLinesAsync(pokemon, ct);
        var hasPrevious = await Pokemons.GetPreviousAsync(pokemon.DexNumber, ct) is not null;
        var hasNext = await Pokemons.GetNextAsync(pokemon.DexNumber, ct) is not null;

        return new PokemonDetails(
            pokemon,
            new StatGauge("ATK", pokemon.Attack.BaseValue, maximums.Attack),
            new StatGauge("DEF", pokemon.Defense.BaseValue, maximums.Defense),
            new StatGauge("STA", pokemon.Stamina.BaseValue, maximums.Stamina),
            moveSet,
            lines
                .Select(line => new EvolutionLineItem(line.Stages
                    .Select((stage, index) => new EvolutionStage(
                        stage,
                        IsFirst: index == 0,
                        IsCurrent: stage.DexNumber == pokemon.DexNumber))
                    .ToArray()))
                .ToArray(),
            hasPrevious,
            hasNext);
    }
}
