using Dex.Core.DataAccess;
using Dex.Core.Entities;
using Dex.Uwp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Dex.Uwp.DataAccess
{
    public class LocalFileDataSource : IPokemonsDataSource, IMovesDataSource, IEvolutionsDataSource, ITypeEffectivenessDataSource
    {
        private const string ContentPrefix = "ms-appx:///";

        private const string EvolutionsDbFilePath = "Data/evolutions.db.json";
        private const string MovesDbFilePath = "Data/moves.db.json";
        private const string PokemonsDbFilePath = "Data/Pokemons.db.json";
        private const string TypesDbFilePath = "Data/types.db.json";

        private readonly IJsonService _jsonService;

        public LocalFileDataSource(IJsonService jsonService)
        {
            _jsonService = jsonService;
        }

        public async Task<List<ushort[]>> LoadAllEvolutionsAsync()
        {
            var json = await ReadFileAsTextAsync(EvolutionsDbFilePath);
            return _jsonService.Deserialize<List<ushort[]>>(json);
        }

        public async Task<PokemonMoves> LoadAllMovesAsync()
        {
            var json = await ReadFileAsTextAsync(MovesDbFilePath);
            return _jsonService.Deserialize<PokemonMoves>(json);
        }

        public async Task<IEnumerable<Pokemon>> LoadAllPokemonsAsync()
        {
            var json = await ReadFileAsTextAsync(PokemonsDbFilePath);
            return _jsonService.Deserialize<List<Pokemon>>(json);
        }

        public async Task<IEnumerable<TypeEffectiveness>> LoadTypeEffectivenessTable()
        {
            var json = await ReadFileAsTextAsync(TypesDbFilePath);
            var result = _jsonService.Deserialize<Dictionary<PokemonType, TypeEffectiveness>>(json);
            return result.ToList()
                .Select(kvp => new TypeEffectiveness(kvp.Value.StrongAgainst, kvp.Value.WeakAgainst, kvp.Key));
        }

        private async Task<string> ReadFileAsTextAsync(string rootUri)
        {
            var uri = new Uri(ContentPrefix + rootUri);
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            return await FileIO.ReadTextAsync(file);
        }
    }
}