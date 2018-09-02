namespace NeoSharp.Persistence.RedisDB.Helpers
{
    public interface IJsonConverter
    {
        T DeserializeObject<T>(string value);

        string SerializeObject(object value);
    }
}
