using NeoSharp.BinarySerialization;
using NeoSharp.Core.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Persistence
{
    public class NeoEntityBase
    {
        [BinaryProperty(0)]
        [JsonProperty("txid")]
        public virtual UInt256 Hash { get; set; }
    }
}