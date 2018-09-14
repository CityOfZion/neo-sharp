using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    public abstract class WitnessBase
    {
        #region Public Properties 
        [JsonProperty("txid")]
        public UInt160 Hash { get; private set; }

        [BinaryProperty(0, MaxLength = 65536)]
        [JsonProperty("invocation")]
        public byte[] InvocationScript { get; private set; }

        [BinaryProperty(1, MaxLength = 65536)]
        [JsonProperty("verification")]
        public byte[] VerificationScript { get; private set; }
        #endregion

        #region Protected Methods 

        protected void Sign(WitnessBase witnessBase)
        {
            this.InvocationScript = witnessBase.InvocationScript;
            this.VerificationScript = witnessBase.VerificationScript;

            this.Hash = new UInt160(Crypto.Default.Hash160(this.VerificationScript));
        }
        #endregion
    }
}
