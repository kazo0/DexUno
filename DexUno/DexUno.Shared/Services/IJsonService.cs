using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dex.Uwp.Services
{
    public interface IJsonService
    {
        T Deserialize<T>(string json);
    }

    public class JsonService : IJsonService
    {
        private JsonSerializerSettings _settings;

        public JsonService()
        {
            _settings = new JsonSerializerSettings();
            _settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}