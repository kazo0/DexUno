using Dex.Core.Base;
using System;

namespace Dex.Core.Entities
{
    public class IV : ValueObject<IV>
    {
        public static readonly IV Max = new IV(15);

        public static readonly IV Min = new IV(0);

        public IV(ushort iv)
        {
            if (iv > 15)
                throw new ArgumentOutOfRangeException($"Iv value ({iv}) is invalid");

            Value = iv;
        }

        public ushort Value { get; }

        protected override bool EqualsCore(IV other) => Value == other.Value;

        protected override int GetHashCodeCore() => Value.GetHashCode();
    }
}