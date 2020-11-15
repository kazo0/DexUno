using Dex.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dex.Core.DataAccess
{
    public interface ITypeEffectivenessDataSource
    {
        Task<IEnumerable<TypeEffectiveness>> LoadTypeEffectivenessTable();
    }
}