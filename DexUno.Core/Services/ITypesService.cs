using System;
using Dex.Core.DataAccess;
using Dex.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dex.Core.Services
{
    public interface ITypesService
    {
        Task<IEnumerable<TypeEffectiveness>> GetAllEffectivnessData();

        IEnumerable<PokemonType> GetAllTypes();

        Task<float> GetTypeAdvantageMultiplier(Move attackingMove, Pokemon defendingPokemon);
    }

    public class TypesService : ITypesService
    {
        private const float NeutralMultiplier = 1f;
        private const float StrongMultiplier = 1.25f;
        private const float WeakMultiplier = 0.8f;

        private readonly ITypeEffectivenessDataSource _typesDataSource;
        private IEnumerable<TypeEffectiveness> _typesEffectivenessMap;

        public TypesService(ITypeEffectivenessDataSource typesDataSource)
        {
            _typesDataSource = typesDataSource;
        }

        public async Task<IEnumerable<TypeEffectiveness>> GetAllEffectivnessData()
        {
            await EnsureDataWasInitialized();
            return _typesEffectivenessMap;
        }

        public IEnumerable<PokemonType> GetAllTypes()
        {
            return Enum.GetValues(typeof(PokemonType)).Cast<PokemonType>().Except(new[] { PokemonType.Unknown });
        }

        public async Task<float> GetTypeAdvantageMultiplier(Move attackingMove, Pokemon defendingPokemon)
        {
            await EnsureDataWasInitialized();

            var damageMultiplier = NeutralMultiplier;

            foreach (var defendingType in defendingPokemon.Types)
            {
                damageMultiplier *= await GetEffectivenessDamage(attackingMove.Type, defendingType);
            }

            return damageMultiplier;
        }

        private async Task EnsureDataWasInitialized()
        {
            if (_typesEffectivenessMap == null)
                _typesEffectivenessMap = await _typesDataSource.LoadTypeEffectivenessTable();
        }

        private TypeEffectiveness GetEffectivenessByType(PokemonType type)
        {
            return _typesEffectivenessMap.First(effectiveness => effectiveness.ConcernedType == type);
        }

        private async Task<float> GetEffectivenessDamage(PokemonType attacker, PokemonType defender)
        {
            await EnsureDataWasInitialized();

            if (GetEffectivenessByType(attacker).StrongAgainst.Contains(defender))
                return StrongMultiplier;

            if (GetEffectivenessByType(attacker).WeakAgainst.Contains(defender))
                return WeakMultiplier;

            return NeutralMultiplier;
        }
    }
}