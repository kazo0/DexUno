using Uno.Extensions.Reactive.Config;

namespace DexUno.Modern.Presentation;

/// <summary>
/// Extends the MVUX-generated view model. The generator passes nested models through untouched, so the
/// wide-layout detail model gets its bindable counterpart here (see <see cref="MoveDetailViewModel.For"/>).
/// </summary>
// The name ends with 'Model', so opt this partial out of MVUX model detection (it is the generated view model itself).
[ReactiveBindable(false)]
public partial class MovedexViewModel
{
    private MoveDetailViewModel? _detail;

    /// <summary>Bindable detail for the master/detail pane. Recreated if hot reload swaps the model.</summary>
    public MoveDetailViewModel Detail
    {
        get
        {
            if (_detail is null || !ReferenceEquals(_detail.Model, Model.DetailModel))
            {
                _detail = MoveDetailViewModel.For(Model.DetailModel);
                RegisterDisposable(_detail);
            }

            return _detail;
        }
    }
}
