using System;
using System.ComponentModel;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    [TypeConverter(typeof(TransactionTypeConverter))]
    public class Transaction : WithHash256
    {
        /// <summary>
        /// Contains the binary output order of the signature for allow to exclude it
        /// </summary>
        private const byte SignatureOrder = byte.MaxValue;

        #region Header

        [BinaryProperty(0)]
        [JsonProperty("type")]
        public TransactionType Type;

        [BinaryProperty(1)]
        [JsonProperty("version")]
        public byte Version;

        #endregion

        // In this point should be serialized the content of the Transaction

        #region TxData

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

        #region Signature

        [BinaryProperty(SignatureOrder)]
        [JsonProperty("scripts")]
        public Witness[] Scripts;

        #endregion

        /// <summary>
        /// Get hash data
        /// </summary>
        /// <returns>Return hash data</returns>
        public override byte[] GetHashData(IBinarySerializer serializer)
        {
            // Exclude signature

            return serializer.Serialize(this, new BinarySerializerSettings()
            {
                Filter = (context => context.Order != SignatureOrder)
            });
        }
    }
}