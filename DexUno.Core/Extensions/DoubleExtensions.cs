using System;

namespace Dex.Core.Extensions
{
    public static class DoubleExtensions
    {
        public static double Pow(this double @base, double exp)
        {
            return Math.Pow(@base, exp);
        }
    }
}