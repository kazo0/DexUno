using Dex.Core.DataAccess;
using Dex.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonType = Dex.Core.Entities.PokemonType;

namespace Dex.Core.Repositories
{
    public interface IMoveRepository
    {
        Task<PokemonMoves> GetAllMoves();

        Task<PokemonMoves> GetAllMovesByType(PokemonType moveType);

        Move GetMoveById(string Id);

        Task<PokemonMoves> GetMovesById(IEnumerable<string> quickMovesIds, IEnumerable<string> chargeMovesIds);
    }

    public class MoveRepository : IMoveRepository
    {
        private readonly IMovesDataSource dataSource;

        private PokemonMoves movesCache;

        public MoveRepository(IMovesDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        public async Task<PokemonMoves> GetAllMoves()
        {
            await EnsureCacheIsValid();
            return movesCache;
        }

        public async Task<PokemonMoves> GetAllMovesByType(PokemonType moveType)
        {
            await EnsureCacheIsValid();
            return new PokemonMoves()
            {
                ChargeMoves = movesCache.ChargeMoves.Where(move => move.Type == moveType).ToList(),
                QuickMoves = movesCache.QuickMoves.Where(move => move.Type == moveType).ToList()
            };
        }

        public Move GetMoveById(string moveId)
        {
            Move foundQuickMove = movesCache.QuickMoves.Where(move => move.MoveId == moveId).SingleOrDefault();
            Move foundChargeMove = movesCache.ChargeMoves.Where(move => move.MoveId == moveId).SingleOrDefault();

            return foundChargeMove ?? foundQuickMove;
        }

        public async Task<PokemonMoves> GetMovesById(IEnumerable<string> quickMovesIds, IEnumerable<string> chargeMovesIds)
        {
            await EnsureCacheIsValid();
            return new PokemonMoves()
            {
                QuickMoves = quickMovesIds.Select(id => GetMoveById(id)).Cast<QuickMove>(),
                ChargeMoves = chargeMovesIds.Select(id => GetMoveById(id)).Cast<ChargeMove>()
            };
        }

        private async Task EnsureCacheIsValid()
        {
            if (movesCache == null)
                movesCache = await dataSource.LoadAllMovesAsync();
        }
    }
}