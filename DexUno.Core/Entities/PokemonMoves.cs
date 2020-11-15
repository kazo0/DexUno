using System.Collections.Generic;

namespace Dex.Core.Entities
{
    public class PokemonMoves
    {
        public IEnumerable<ChargeMove> ChargeMoves { get; set; }
        public IEnumerable<QuickMove> QuickMoves { get; set; }
    }

    public class PokemonMovesIds
    {
        public IEnumerable<string> ChargeMovesIds { get; set; }
        public IEnumerable<string> QuickMovesIds { get; set; }
    }
}