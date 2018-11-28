namespace NeoSharp.Core.Converters
{
    public interface IJsonConverter
    {
        T DeserializeObject<T>(string value);
        string SerializeObject(object value);
    }
}
