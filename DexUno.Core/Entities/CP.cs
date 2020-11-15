using Dex.Core.Base;
using Dex.Core.Extensions;
using System;

namespace Dex.Core.Entities
{
    public class CP : ValueObject<CP>, IComparable<CP>
    {
        public CP(double value)
        {
            PreciseValue = value;
            Value = ((ushort)Math.Floor(PreciseValue)).ClipToMin(10);
        }

        public double PreciseValue { get; }

        public ushort Value { get; }

        public int CompareTo(CP other)
        {
            return Value.CompareTo(other.Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        protected override bool EqualsCore(CP other)
        {
            return Value == other.Value;
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }
    }
}