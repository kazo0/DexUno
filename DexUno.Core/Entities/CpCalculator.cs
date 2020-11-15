using Dex.Core.Extensions;
using System;

using CpMap = Dex.Core.Entities.CpMultipliersMap;

namespace Dex.Core.Entities
{
    public class CpCalculator
    {
        private CombatStat _attack;
        private CombatStat _defense;
        private CombatStat _stamina;

        public CpCalculator(CombatStat attack, CombatStat defense, CombatStat stamina)
        {
            _attack = attack ?? throw new ArgumentNullException(nameof(attack));
            _defense = defense ?? throw new ArgumentNullException(nameof(defense));
            _stamina = stamina ?? throw new ArgumentNullException(nameof(stamina));
        }

        public CP Max => MaxAtLevel(40f);

        public CP Min => MinAtLevel(1.0f);

        public CP AtLevel(float level)
        {
            return Calculate(_attack.Value, _defense.Value, _stamina.Value, level);
        }

        public CP MaxAtLevel(float level)
        {
            return Calculate(_attack.Max, _defense.Max, _stamina.Max, level);
        }

        public CP MinAtLevel(float level)
        {
            return Calculate(_attack.Min, _defense.Min, _stamina.Min, level);
        }

        private static CP Calculate(ushort attack, ushort defense, ushort stamina, float pokemonLevel)
        {
            if (!CpMap.LevelToCpm.ContainsKey(pokemonLevel))
                throw new ArgumentException($"Invalid Pokemon level ({pokemonLevel})");

            var calculatedCp = attack * defense.Pow(0.5) * stamina.Pow(0.5) * CpMap.LevelToCpm[pokemonLevel].Pow(2) / 10;

            return new CP(calculatedCp);
        }
    }
}