using System;

namespace Dex.Uwp.ViewModels
{
    public class CombatStatDataViewModel
    {
        public CombatStatDataViewModel(ushort statValue, ushort maxStatValue)
        {
            MaxStatValue = maxStatValue;
            StatValue = statValue;
            Percentage = (ushort)Math.Round((double)(100 * StatValue) / MaxStatValue);
        }

        public ushort MaxStatValue { get; }
        public ushort Percentage { get; }
        public ushort StatValue { get; }
    }
}