using Dex.Core.DataAccess;
using Dex.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dex.Core.Repositories
{
    public interface IPokemonRepository
    {
        Task<IEnumerable<Pokemon>> GetAllPokemons();

        Task<ushort> GetMaxBaseAttack();

        Task<ushort> GetMaxBaseDefense();

        Task<ushort> GetMaxBaseStamina();

        Task<Pokemon> GetNextPokemon(ushort PokemonId);

        Task<Pokemon> GetPokemonById(ushort pokemonId);

        Task<IEnumerable<List<Pokemon>>> GetPokemonsFromEvolutionLine(List<ushort[]> evolutionLines);

        Task<IEnumerable<Pokemon>> GetPokemonsWithMove(string moveId);

        Task<Pokemon> GetPreviousPokemon(ushort PokemonId);

        bool HasNextPokemon(ushort PokemonId);

        bool HasPreviousPokemon(ushort PokemonId);
    }

    public class PokemonRepository : IPokemonRepository
    {
        private readonly IPokemonsDataSource dataSource;

        private List<Pokemon> allPokemonsCache;

        private CombatStat maxAttack;
        private CombatStat maxDefense;
        private ushort maxDexNumber;
        private CombatStat maxStamina;
        private ushort minDexNumber = 1;

        public PokemonRepository(IPokemonsDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        public async Task<IEnumerable<Pokemon>> GetAllPokemons()
        {
            await EnsureCacheIsValid();
            return allPokemonsCache;
        }

        public async Task<ushort> GetMaxBaseAttack()
        {
            await EnsureCacheIsValid();
            maxAttack = maxAttack ?? allPokemonsCache.Max(poke => poke.Attack);
            return maxAttack.Value;
        }

        public async Task<ushort> GetMaxBaseDefense()
        {
            await EnsureCacheIsValid();
            maxDefense = maxDefense ?? allPokemonsCache.Max(poke => poke.Defense);
            return maxDefense.Value;
        }

        public async Task<ushort> GetMaxBaseStamina()
        {
            await EnsureCacheIsValid();
            maxStamina = maxStamina ?? allPokemonsCache.Max(poke => poke.Stamina);
            return maxStamina.Value;
        }

        public async Task<Pokemon> GetNextPokemon(ushort PokemonId)
        {
            await EnsureCacheIsValid();
            var currentPokemon = await GetPokemonById((PokemonId));
            var currentPokemonIndex = allPokemonsCache.IndexOf(currentPokemon);
            return allPokemonsCache[currentPokemonIndex + 1];
        }

        public async Task<Pokemon> GetPokemonById(ushort pokemonId)
        {
            await EnsureCacheIsValid();
            return allPokemonsCache
                .Where(pokemon => pokemon.DexNumber == pokemonId)
                .FirstOrDefault();
        }

        public async Task<Pokemon> GetPokemonByName(string pokemonName)
        {
            await EnsureCacheIsValid();
            return allPokemonsCache
                .Where(pokemon => pokemon.Name == pokemonName)
                .FirstOrDefault();
        }

        public async Task<IEnumerable<List<Pokemon>>> GetPokemonsFromEvolutionLine(List<ushort[]> evolutionLines)
        {
            List<List<Pokemon>> returnList = new List<List<Pokemon>>();

            foreach (var line in evolutionLines)
            {
                var array = await Task.WhenAll(line.Select(dexId => GetPokemonById(dexId)));
                returnList.Add(array.ToList());
            }

            return returnList;
        }

        public async Task<IEnumerable<Pokemon>> GetPokemonsWithMove(string moveId)
        {
            await EnsureCacheIsValid();
            return allPokemonsCache.Where(poke => poke.Moves.ChargeMovesIds.Contains(moveId) || poke.Moves.QuickMovesIds.Contains(moveId));
        }

        public async Task<Pokemon> GetPreviousPokemon(ushort PokemonId)
        {
            await EnsureCacheIsValid();
            var currentPokemon = await GetPokemonById((PokemonId));
            var currentPokemonIndex = allPokemonsCache.IndexOf(currentPokemon);
            return allPokemonsCache[currentPokemonIndex - 1];
        }

        public bool HasNextPokemon(ushort PokemonId)
        {
            return PokemonId < maxDexNumber;
        }

        public bool HasPreviousPokemon(ushort PokemonId)
        {
            return PokemonId > minDexNumber;
        }

        private async Task EnsureCacheIsValid()
        {
            if (allPokemonsCache == null)
            {
                allPokemonsCache = (await dataSource.LoadAllPokemonsAsync()).ToList();
                maxDexNumber = allPokemonsCache.Max(poke => poke.DexNumber);
            }
        }
    }
}