using System.Collections.Generic;
using System.Linq;

namespace Dex.Core.Entities
{
    public class TypeEffectiveness
    {
        public TypeEffectiveness(IEnumerable<PokemonType> strengths, IEnumerable<PokemonType> weaknesses, PokemonType concernedType)
        {
            StrongAgainst = strengths.ToArray();
            WeakAgainst = weaknesses.ToArray();
            ConcernedType = concernedType;
        }

        public PokemonType ConcernedType { get; }

        public PokemonType[] StrongAgainst { get; }

        public PokemonType[] WeakAgainst { get; }
    }
}