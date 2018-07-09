using System;
namespace NeoSharp.Core.Wallet.Wrappers
{
    public interface IJsonConverter
    {
        T DeserializeObject<T>(string value);
        string SerializeObject(object value);
    }
}
