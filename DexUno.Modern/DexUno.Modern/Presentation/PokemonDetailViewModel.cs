using Uno.Extensions.Reactive.Config;

namespace DexUno.Modern.Presentation;

// The name ends with 'Model', so opt this partial out of MVUX model detection (it is the generated view model itself).
[ReactiveBindable(false)]
public partial class PokemonDetailViewModel
{
    /// <summary>Wraps a model owned by another model (the Pokédex master/detail pane) through the generated model constructor.</summary>
    internal static PokemonDetailViewModel For(PokemonDetailModel model) => new(model);
}
