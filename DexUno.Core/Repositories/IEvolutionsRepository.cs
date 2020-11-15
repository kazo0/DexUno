using Dex.Core.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dex.Core.Repositories
{
    public interface IEvolutionsRepository
    {
        Task<List<ushort[]>> GetEvolutionLines(ushort dexId);

        Task<bool> HasEvolutions(ushort dexId);
    }

    public class EvolutionsRepository : IEvolutionsRepository
    {
        private readonly IEvolutionsDataSource _evolutionsDataSource;
        private List<ushort[]> _allEvolutionsCache;

        public EvolutionsRepository(IEvolutionsDataSource evolutionsDataSource)
        {
            _evolutionsDataSource = evolutionsDataSource;
        }

        public async Task<List<ushort[]>> GetEvolutionLines(ushort dexId)
        {
            await EnsureCacheIsValid();
            return _allEvolutionsCache.Where(line => line.Contains(dexId)).ToList();
        }

        public async Task<bool> HasEvolutions(ushort dexId)
        {
            await EnsureCacheIsValid();
            return _allEvolutionsCache.Where(line => line.Contains(dexId) && line.Count() > 1).Any();
        }

        private async Task EnsureCacheIsValid()
        {
            if (_allEvolutionsCache == null)
            {
                _allEvolutionsCache = await _evolutionsDataSource.LoadAllEvolutionsAsync();
            }
        }
    }
}