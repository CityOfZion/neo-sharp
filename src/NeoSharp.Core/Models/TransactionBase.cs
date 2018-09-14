using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    public abstract class TransactionBase
    {
        #region Public Properties 
        [BinaryProperty(0)]
        [JsonProperty("hash")]
        public UInt256 Hash { get; private set; }

        [BinaryProperty(1)]
        [JsonProperty("type")]
        public TransactionType Type { get; private set; }

        [BinaryProperty(2)]
        [JsonProperty("version")]
        public byte Version { get; private set; }

        [BinaryProperty(100)]
        [JsonProperty("attributes")]
        public TransactionAttribute[] Attributes = new TransactionAttribute[0];

        [BinaryProperty(101)]
        [JsonProperty("vin")]
        public CoinReference[] Inputs = new CoinReference[0];

        [BinaryProperty(102)]
        [JsonProperty("vout")]
        public TransactionOutput[] Outputs = new TransactionOutput[0];
        #endregion

        #region Protected Methods 
        public void Sign(TransactionBase transactionBase, byte[] signingSettings)
        {
            this.Type = transactionBase.Type;
            this.Version = transactionBase.Version;
            this.Attributes = transactionBase.Attributes;
            this.Inputs = transactionBase.Inputs;
            this.Outputs = transactionBase.Outputs;

            this.Hash = new UInt256(Crypto.Default.Hash256(signingSettings));
        }
        #endregion
    }
}
