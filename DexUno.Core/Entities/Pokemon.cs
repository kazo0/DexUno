using System;
using System.Diagnostics;

namespace Dex.Core.Entities
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class Pokemon
    {
        private CpCalculator _cp;

        #region General

        public ushort DexNumber { get; set; }

        public string Name { get; set; }

        public PokemonType[] Types { get; set; }

        #endregion General

        #region CombatStats

        public CombatStat Attack { get; set; }

        public CpCalculator Cp => _cp ?? (_cp = new CpCalculator(Attack, Defense, Stamina));

        public CombatStat Defense { get; set; }

        public CombatStat Stamina { get; set; }

        #endregion CombatStats

        #region GeneralStats

        public ushort CatchRate { get; set; }

        public ushort FleeRate { get; set; }

        #endregion GeneralStats

        #region Moves

        public PokemonMovesIds Moves { get; set; }

        #endregion Moves

        #region Other

        public ushort CandiesToEvolve { get; set; }

        public ushort EggDistance { get; set; }

        #endregion Other
    }
}