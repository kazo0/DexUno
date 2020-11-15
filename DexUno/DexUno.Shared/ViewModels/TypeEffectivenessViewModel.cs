using Dex.Core.Entities;
using Dex.Core.Services;
using Dex.Uwp.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Dex.Uwp.ViewModels
{
    public class TypeEffectivenessViewModel : ViewModelBase
    {
        private readonly ITypesService _typeService;
        private IEnumerable<TypeEffectiveness> _effectivenessData;

        public TypeEffectivenessViewModel(ITypesService typeService)
        {
            _typeService = typeService;
        }

        public IEnumerable<TypeEffectiveness> EffectivenessData
        {
            get { return _effectivenessData; }
            private set { Set(ref _effectivenessData, value); }
        }

        public override async Task OnNavigatedTo(NavigationEventArgs e)
        {
            EffectivenessData = await _typeService.GetAllEffectivnessData();
        }
    }
}