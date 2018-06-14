using NeoSharp.BinarySerialization;
using NeoSharp.Core.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Persistence
{
    public class NeoEntityBase
    {
        [JsonProperty("txid")]
        public virtual UInt256 Hash { get; set; }
    }
}
