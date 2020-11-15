using Dex.Uwp.Helpers;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using PokemonType = Dex.Core.Entities.PokemonType;

namespace Dex.Uwp.ValueConverters
{
    public class PokemonTypeToColorConverter : IValueConverter
    {
        private readonly Dictionary<ushort, SolidColorBrush> TypeToColorMap;

        public PokemonTypeToColorConverter()
        {
            TypeToColorMap = new Dictionary<ushort, SolidColorBrush>()
            {
                {(ushort)PokemonType.Bug,      ColorUtils.BrushFromHex("#a8b820") },
                {(ushort)PokemonType.Grass,    ColorUtils.BrushFromHex("#78c850") },
                {(ushort)PokemonType.Dark,     ColorUtils.BrushFromHex("#705848") },
                {(ushort)PokemonType.Ground,   ColorUtils.BrushFromHex("#e0c068") },
                {(ushort)PokemonType.Dragon,   ColorUtils.BrushFromHex("#7038f8") },
                {(ushort)PokemonType.Ice,      ColorUtils.BrushFromHex("#98d8d8") },
                {(ushort)PokemonType.Electric, ColorUtils.BrushFromHex("#f8d030") },
                {(ushort)PokemonType.Normal,   ColorUtils.BrushFromHex("#8a8a59") },
                {(ushort)PokemonType.Fairy,    ColorUtils.BrushFromHex("#e898e8") },
                {(ushort)PokemonType.Poison,   ColorUtils.BrushFromHex("#a040a0") },
                {(ushort)PokemonType.Fighting, ColorUtils.BrushFromHex("#c03028") },
                {(ushort)PokemonType.Psychic,  ColorUtils.BrushFromHex("#f85888") },
                {(ushort)PokemonType.Fire,     ColorUtils.BrushFromHex("#f08030") },
                {(ushort)PokemonType.Rock,     ColorUtils.BrushFromHex("#b8a038") },
                {(ushort)PokemonType.Flying,   ColorUtils.BrushFromHex("#a890f0") },
                {(ushort)PokemonType.Steel,    ColorUtils.BrushFromHex("#b8b8d0") },
                {(ushort)PokemonType.Ghost,    ColorUtils.BrushFromHex("#705898") },
                {(ushort)PokemonType.Water,    ColorUtils.BrushFromHex("#6890f0") },
                {(ushort)PokemonType.Unknown,  ColorUtils.BrushFromHex("#8a8a59")  }
            };
        }

        public object Convert(object value, System.Type targetType, object parameter, string language)
        {
            PokemonType type = (PokemonType)value;
            return TypeToColorMap[(ushort)type];
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}