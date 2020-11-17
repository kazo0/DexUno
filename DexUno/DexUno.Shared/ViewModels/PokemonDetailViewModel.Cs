using Dex.Core.Entities;
using Dex.Core.Repositories;
using Dex.Uwp.Infrastructure;
using Dex.Uwp.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace Dex.Uwp.ViewModels
{
    [Bindable]
    public class PokemonDetailViewModel : ViewModelBase
    {
        private readonly IEvolutionsRepository _evolutionRepository;
        private readonly IMoveRepository _moveRepository;
        private readonly INavigationService _navigationService;
		private readonly ILogger _logger;
		private readonly IPokemonRepository _pokemonRepository;

        private CombatStatDataViewModel attackStats;
        private CombatStatDataViewModel defenseStats;
        private EvolutionLinesViewModel evolutionLines;
        private PokemonMoves moves;
        private Pokemon selectedPokemon;
        private CombatStatDataViewModel staminaStats;

        public PokemonDetailViewModel(
            IPokemonRepository pokemonRepository,
            IMoveRepository moveRepository,
            IEvolutionsRepository evolutionRepository,
            INavigationService navigationService,
            ILogger logger)
        {
            _evolutionRepository = evolutionRepository;
            _navigationService = navigationService;
			_logger = logger;
			_moveRepository = moveRepository;
            _pokemonRepository = pokemonRepository;

            PreviousCommand = new RelayCommand(async () => await SetNewPokemon(await pokemonRepository.GetPreviousPokemon(SelectedPokemon.DexNumber)), () => pokemonRepository.HasPreviousPokemon(SelectedPokemon?.DexNumber?? 0));
            NextCommand = new RelayCommand(async () => await SetNewPokemon(await pokemonRepository.GetNextPokemon(SelectedPokemon.DexNumber)), () => pokemonRepository.HasNextPokemon(SelectedPokemon?.DexNumber ?? 0));
        }

        public CombatStatDataViewModel AttackStats
        {
            get { return attackStats; }
            private set { Set(ref attackStats, value); }
        }

        public CombatStatDataViewModel DefenseStats
        {
            get { return defenseStats; }
            private set { Set(ref defenseStats, value); }
        }

        public EvolutionLinesViewModel EvolutionLines
        {
            get { return evolutionLines; }
            private set { Set(ref evolutionLines, value); }
        }

        public PokemonMoves Moves
        {
            get { return moves; }
            private set { Set(ref moves, value); }
        }

        public RelayCommand NextCommand { get; }

        public RelayCommand PreviousCommand { get; }

        public Pokemon SelectedPokemon
        {
            get { return selectedPokemon; }
            private set { Set(ref selectedPokemon, value); }
        }

        public CombatStatDataViewModel StaminaStats
        {
            get { return staminaStats; }
            private set { Set(ref staminaStats, value); }
        }

        public void OnMoveSelected(Move move)
        {
            _navigationService.ResolveFromThenNavigate(move);
        }

        public async override Task OnNavigatedTo(NavigationEventArgs e)
        {
            var dexId = (ushort)e.Parameter;
            await SetPokemon(dexId);
            _logger.LogDebug($"Got Pokemon {SelectedPokemon.Name}");
        }

        public async Task SetPokemon(ushort dexId)
        {
            _logger.LogDebug($"Fetch pokemon for ID {dexId}");
            await SetNewPokemon(await _pokemonRepository.GetPokemonById(dexId));
            _logger.LogDebug($"Got Pokemon {SelectedPokemon.Name}");
        }


        private async Task InitCombatStats()
        {
            var maxAtk = _pokemonRepository.GetMaxBaseAttack();
            var maxDef = _pokemonRepository.GetMaxBaseDefense();
            var maxStm = _pokemonRepository.GetMaxBaseStamina();
            await Task.WhenAll(new Task[] { maxAtk, maxDef, maxStm });

            AttackStats = new CombatStatDataViewModel(SelectedPokemon.Attack.Value, maxAtk.Result);
            DefenseStats = new CombatStatDataViewModel(SelectedPokemon.Defense.Value, maxDef.Result);
            StaminaStats = new CombatStatDataViewModel(SelectedPokemon.Stamina.Value, maxStm.Result);
        }

        private async Task SetNewPokemon(Pokemon newPokemon)
        {
            SelectedPokemon = newPokemon;
            Moves = await _moveRepository.GetMovesById(SelectedPokemon.Moves.QuickMovesIds, SelectedPokemon.Moves.ChargeMovesIds);

            IEnumerable<EvolutionLineDataViewModel> foundEvolutionLines = Enumerable.Empty<EvolutionLineDataViewModel>();

            if (await _evolutionRepository.HasEvolutions(selectedPokemon.DexNumber))
            {
                var evolutionLinesWithIds = await _evolutionRepository.GetEvolutionLines(selectedPokemon.DexNumber);
                foundEvolutionLines = (await _pokemonRepository.GetPokemonsFromEvolutionLine(evolutionLinesWithIds))
                    .Select(line => new EvolutionLineDataViewModel(line));
            }

            EvolutionLines = new EvolutionLinesViewModel(foundEvolutionLines);
            await InitCombatStats();
            NextCommand.RaiseCanExecuteChanged();
            PreviousCommand.RaiseCanExecuteChanged();
        }
    }
}