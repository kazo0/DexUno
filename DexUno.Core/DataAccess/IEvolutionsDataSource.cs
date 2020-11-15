using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dex.Core.DataAccess
{
    public interface IEvolutionsDataSource
    {
        Task<List<ushort[]>> LoadAllEvolutionsAsync();
    }
}