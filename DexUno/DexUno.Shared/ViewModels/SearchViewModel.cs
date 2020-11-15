using Dex.Core.Entities;
using Dex.Core.Repositories;
using Dex.Uwp.Infrastructure;
using Dex.Uwp.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Dex.Uwp.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private readonly IMoveRepository moveRepository;
        private readonly INavigationService navigationService;
        private readonly IPokemonRepository pokemonsRepository;

        private PokemonMoves allMovesCache;
        private IEnumerable<Pokemon> allPokemonsCache;
        private string searchQuery;
        private IEnumerable<object> searchResult;

        public SearchViewModel(IPokemonRepository pokemonsRepository, IMoveRepository moveRepository, INavigationService navigationService)
        {
            this.navigationService = navigationService;
            this.moveRepository = moveRepository;
            this.pokemonsRepository = pokemonsRepository;
        }

        public string SearchQuery
        {
            get { return searchQuery; }
            set
            {
                searchQuery = value;
                OnNewSearch(searchQuery);
            }
        }

        public IEnumerable<object> SearchResult
        {
            get { return searchResult; }
            set { Set(ref searchResult, value); }
        }

        public void OnItemChosen(object SelectedItem)
        {
            navigationService.ResolveFromThenNavigate(SelectedItem);
        }

        public async override Task OnNavigatedTo(NavigationEventArgs e)
        {
            var pokes = pokemonsRepository.GetAllPokemons();
            var moves = moveRepository.GetAllMoves();

            allPokemonsCache = await pokemonsRepository.GetAllPokemons();
            allMovesCache = await moveRepository.GetAllMoves();
        }

        private void OnNewSearch(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                SearchResult = null;
                return;
            }

            query = query.ToLowerInvariant();
            var foundPokes = allPokemonsCache.Where(poke => poke.Name.ToLowerInvariant().Contains(query));
            var foundQuickMoves = allMovesCache.QuickMoves.Where(move => move.Name.ToLowerInvariant().Contains(query));
            var foundChargeMoves = allMovesCache.ChargeMoves.Where(move => move.Name.ToLowerInvariant().Contains(query));

            var resultList = new List<object>();
            resultList.AddRange(foundPokes);
            resultList.AddRange(foundQuickMoves);
            resultList.AddRange(foundChargeMoves);

            SearchResult = resultList;
        }
    }
}