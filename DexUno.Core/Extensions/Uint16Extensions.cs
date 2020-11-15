using System;
using System.Linq;

namespace Dex.Core.Extensions
{
    public static class UInt16Extensions
    {
        public static ushort ClipToMin(this ushort numberToClip, ushort minBoundary)
        {
            return Math.Max(minBoundary, numberToClip);
        }

        public static double Pow(this ushort @base, double exp)
        {
            return Math.Pow(@base, exp);
        }
    }
}