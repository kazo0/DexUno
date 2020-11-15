using Dex.Core.Entities;
using Dex.Core.Repositories;
using Dex.Uwp.Infrastructure;
using Dex.Uwp.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace Dex.Uwp.ViewModels
{
    [Bindable]
    public class PokedexViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IPokemonRepository _pokemonsRepository;

        private IEnumerable<Pokemon> _allPokemonsByCp;
        private IEnumerable<Pokemon> _allPokemonsByDexNumber;
        private IEnumerable<Pokemon> _allPokemonsByName;
        private IEnumerable<Pokemon> _allPokemonsByType;
        private IEnumerable<Pokemon> _allPokemonsCache;

        public PokedexViewModel(IPokemonRepository pokemonsRepository, INavigationService navigationService)
        {
            this._navigationService = navigationService;
            this._pokemonsRepository = pokemonsRepository;

            ReverseOrderCommand = new RelayCommand(OnReverseOrder);
            SearchCommand = new RelayCommand(navigationService.NavigateToSearchPage);
        }

        public IEnumerable<Pokemon> AllPokemonsByCp
        {
            get { return _allPokemonsByCp; }
            private set { Set(ref _allPokemonsByCp, value); }
        }

        public IEnumerable<Pokemon> AllPokemonsByDexNumber
        {
            get { return _allPokemonsByDexNumber; }
            private set { Set(ref _allPokemonsByDexNumber, value); }
        }

        public IEnumerable<Pokemon> AllPokemonsByName
        {
            get { return _allPokemonsByName; }
            private set { Set(ref _allPokemonsByName, value); }
        }

        public IEnumerable<Pokemon> AllPokemonsByType
        {
            get { return _allPokemonsByType; }
            private set { Set(ref _allPokemonsByType, value); }
        }

        public ICommand ReverseOrderCommand { get; }
        public ICommand SearchCommand { get; }

        public override async Task OnNavigatedTo(NavigationEventArgs e)
        {
            var pokes = _pokemonsRepository.GetAllPokemons();
            _allPokemonsCache = (await pokes).ToList();

            AllPokemonsByDexNumber = _allPokemonsCache.OrderBy(poke => poke.DexNumber);
            AllPokemonsByCp = _allPokemonsCache.OrderBy(poke => poke.Cp.Max);
            AllPokemonsByName = _allPokemonsCache.OrderBy(poke => poke.Name);
            AllPokemonsByType = _allPokemonsCache.OrderBy(poke => poke.Types[0]);
        }

        public void OnSelectNewItem(Pokemon selectedPoke)
        {
            _navigationService.NavigateToPokemonDetailsPage(selectedPoke.DexNumber);
        }

        private void OnReverseOrder()
        {
            AllPokemonsByCp = AllPokemonsByCp.Reverse();
            AllPokemonsByDexNumber = AllPokemonsByDexNumber.Reverse();
            AllPokemonsByName = AllPokemonsByName.Reverse();
            AllPokemonsByType = AllPokemonsByType.Reverse();
        }
    }
}