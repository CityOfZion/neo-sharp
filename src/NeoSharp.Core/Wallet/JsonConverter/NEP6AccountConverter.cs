using System;
using NeoSharp.Core.Wallet.NEP6;
using Newtonsoft.Json;

namespace NeoSharp.Core.Wallet
{
    public class NEP6AccountConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<NEP6Account[]>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
