using System;
using System.IO;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;
using NeoSharp.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class Transaction 
    {
        #region Header

        [BinaryProperty(0)]
        [JsonProperty("hash")]
        public UInt256 Hash { get; set; }

        [BinaryProperty(1)]
        [JsonProperty("type")]
        public readonly TransactionType Type;

        [BinaryProperty(2)]
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

        [BinaryProperty(255)]
        [JsonProperty("witness")]
        public Witness[] Witness;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Transaction()
        {
            // Just for serialization
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        protected Transaction(TransactionType type)
        {
            Type = type;
        }

        #region Serialization

        /// <summary>
        /// Deserialize logic
        /// </summary>
        /// <param name="deserializer">Deserializer</param>
        /// <param name="reader">Reader</param>
        /// <param name="settings">Settings</param>
        public void Deserialize(IBinarySerializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
        {
            // Check type

            // Byte already readed

            // if ((byte)Type != reader.ReadByte())
            //    throw new FormatException();

            // Read version

            Version = reader.ReadByte();

            // Deserialize exclusive data

            DeserializeExclusiveData(deserializer, reader, settings);

            // Deserialize shared content

            Attributes = deserializer.Deserialize<TransactionAttribute[]>(reader, settings);
            if (Attributes.Length > ushort.MaxValue) throw new FormatException(nameof(Attributes));

            Inputs = deserializer.Deserialize<CoinReference[]>(reader, settings);
            if (Inputs.Length > ushort.MaxValue) throw new FormatException(nameof(Inputs));

            Outputs = deserializer.Deserialize<TransactionOutput[]>(reader, settings);
            if (Outputs.Length > ushort.MaxValue) throw new FormatException(nameof(Outputs));

            // Deserialize signature

            if (settings?.Filter?.Invoke(nameof(Witness)) != false)
            {
                Witness = deserializer.Deserialize<Witness[]>(reader, settings);
                if (Witness.Length > ushort.MaxValue) throw new FormatException(nameof(Witness));
            }
        }

        /// <summary>
        /// Serialize logic
        /// </summary>
        /// <param name="serializer">Serializer</param>
        /// <param name="writer">Writer</param>
        /// <param name="settings">Settings</param>
        /// <returns>How many bytes have been written</returns>
        public int Serialize(IBinarySerializer serializer, BinaryWriter writer, BinarySerializerSettings settings = null)
        {
            // Write type and version

            var ret = 2;

            writer.Write((byte)Type);
            writer.Write(Version);

            // Serialize exclusive data

            ret += SerializeExclusiveData(serializer, writer, settings);

            // Serialize shared content

            ret += serializer.Serialize(Attributes, writer, settings);
            ret += serializer.Serialize(Inputs, writer, settings);
            ret += serializer.Serialize(Outputs, writer, settings);

            // Serialize sign

            if (settings?.Filter?.Invoke(nameof(Witness)) != false)
            {
                ret += serializer.Serialize(Witness, writer, settings);
            }

            return ret;
        }

        /// <summary>
        /// Deserialize logic
        /// </summary>
        /// <param name="deserializer">Deserializer</param>
        /// <param name="reader">Reader</param>
        /// <param name="settings">Settings</param>
        /// <returns>How many bytes have been written</returns>
        protected virtual void DeserializeExclusiveData(IBinarySerializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
        {

        }

        /// <summary>
        /// Serialize logic
        /// </summary>
        /// <param name="serializer">Serializer</param>
        /// <param name="writer">Writer</param>
        /// <param name="settings">Settings</param>
        /// <returns>How many bytes have been written</returns>
        protected virtual int SerializeExclusiveData(IBinarySerializer serializer, BinaryWriter writer, BinarySerializerSettings settings = null)
        {
            return 0;
        }

        #endregion
    }
}