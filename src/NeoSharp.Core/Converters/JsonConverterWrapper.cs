using System;
using Newtonsoft.Json;

namespace NeoSharp.Core.Wallet.Wrappers
{
    public class JsonConverterWrapper : IJsonConverter
    {
        public T DeserializeObject<T>(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                throw new ArgumentException(nameof(jsonString));
            }

            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public string SerializeObject(object value){
            var internalObject = value ?? throw new ArgumentNullException(nameof(value));
            return JsonConvert.SerializeObject(value);
        }
    }
}