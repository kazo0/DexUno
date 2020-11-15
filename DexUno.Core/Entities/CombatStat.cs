using Dex.Core.Base;
using System;

namespace Dex.Core.Entities
{
    public class CombatStat : ValueObject<CombatStat>, IComparable<CombatStat>
    {
        public CombatStat(ushort baseValue, IV iv = null)
        {
            Iv = iv ?? IV.Min;
            BaseValue = baseValue;
        }

        public ushort BaseValue { get; set; }

        public IV Iv { get; }

        public ushort Max => (ushort)(BaseValue + IV.Max.Value);

        public ushort Min => (ushort)(BaseValue + IV.Min.Value);

        public ushort Value => (ushort)(BaseValue + Iv.Value);

        public int CompareTo(CombatStat that)
        {
            return (BaseValue + Iv.Value).CompareTo(that.BaseValue + that.Iv.Value);
        }

        protected override bool EqualsCore(CombatStat other)
        {
            return BaseValue == other.BaseValue && Iv == other.Iv;
        }

        protected override int GetHashCodeCore()
        {
            return BaseValue.GetHashCode() ^ Iv.GetHashCode();
        }
    }
}