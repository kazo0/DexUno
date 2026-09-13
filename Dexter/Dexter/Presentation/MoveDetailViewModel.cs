using Uno.Extensions.Reactive.Config;

namespace Dexter.Presentation;

// The name ends with 'Model', so opt this partial out of MVUX model detection (it is the generated view model itself).
[ReactiveBindable(false)]
public partial class MoveDetailViewModel
{
    /// <summary>Wraps a model owned by another model (the Movedex master/detail pane) through the generated model constructor.</summary>
    internal static MoveDetailViewModel For(MoveDetailModel model) => new(model);
}
