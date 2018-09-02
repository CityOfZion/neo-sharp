using Newtonsoft.Json;

namespace NeoSharp.Persistence.RedisDB.Helpers
{
    public class JsonConverter : IJsonConverter
    {
        public T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
