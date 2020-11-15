using Dex.Core.Entities;
using Dex.Core.Repositories;
using Dex.Uwp.Infrastructure;
using Dex.Uwp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;

namespace Dex.Uwp.ViewModels
{
    public class MovedexViewModel : ViewModelBase
    {
        private readonly IMoveRepository moveRepository;
        private readonly INavigationService navigationService;
        private IEnumerable<ChargeMove> allChargeMovesById;
        private PokemonMoves allMoves;

        private IEnumerable<QuickMove> allQuickMovesById;

        public MovedexViewModel(IMoveRepository moveRepository, INavigationService navigationService)
        {
            this.navigationService = navigationService;
            this.moveRepository = moveRepository;
            ReverseOrderCommand = new RelayCommand(() => OnReverseOrder());
        }

        public IEnumerable<ChargeMove> AllChargeMovesById
        {
            get { return allChargeMovesById; }
            private set { Set(ref allChargeMovesById, value); }
        }

        public IEnumerable<QuickMove> AllQuickMovesById
        {
            get { return allQuickMovesById; }
            private set { Set(ref allQuickMovesById, value); }
        }

        public ICommand ReverseOrderCommand { get; }

        public void OnMoveSelected(Move selectedMove)
        {
            navigationService.NavigateToMoveDetailsPage(selectedMove.MoveId);
        }

        public async override Task OnNavigatedTo(NavigationEventArgs e)
        {
            allMoves = await moveRepository.GetAllMoves();
            AllChargeMovesById = allMoves.ChargeMoves;
            AllQuickMovesById = allMoves.QuickMoves;
        }

        private void OnReverseOrder()
        {
            AllChargeMovesById = AllChargeMovesById.Reverse();
            AllQuickMovesById = AllQuickMovesById.Reverse();
        }
    }
}