using Dex.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Dex.Uwp.ViewModels
{
    public class EvolutionLineDataViewModel
    {
        public EvolutionLineDataViewModel(IEnumerable<Pokemon> evolutionLine)
        {
            Evolutions = evolutionLine.ToList();
            NumberOfEvolutions = !evolutionLine.Any() ? 1 : evolutionLine.Count();
        }

        public IEnumerable<Pokemon> Evolutions { get; }
        public int NumberOfEvolutions { get; }
    }

    public class EvolutionLinesViewModel
    {
        public EvolutionLinesViewModel(IEnumerable<EvolutionLineDataViewModel> evolutionLines)
        {
            EvolutionLines = evolutionLines;
        }

        public IEnumerable<EvolutionLineDataViewModel> EvolutionLines { get; }
    }
}