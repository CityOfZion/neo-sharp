using System;
using System.IO;
using NeoSharp.BinarySerialization;
using NeoSharp.BinarySerialization.SerializationHooks;
using NeoSharp.Core.Converters;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class Transaction : WithHash256, IBinaryVerifiable
    {
        /// <summary>
        /// Contains the binary output order of the signature for allow to exclude it
        /// </summary>
        private const int SignatureOrder = int.MaxValue;

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

        [BinaryProperty(SignatureOrder - 3)]
        [JsonProperty("attributes")]
        public TransactionAttribute[] Attributes = new TransactionAttribute[0];

        [BinaryProperty(SignatureOrder - 2)]
        [JsonProperty("vin")]
        public CoinReference[] Inputs = new CoinReference[0];

        [BinaryProperty(SignatureOrder - 1)]
        [JsonProperty("vout")]
        public TransactionOutput[] Outputs = new TransactionOutput[0];

        #endregion

        #region Signatures

        [BinaryProperty(SignatureOrder)]
        [JsonProperty("scripts")]
        public Witness[] Scripts;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Transaction() { }

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
        public void Deserialize(IBinaryDeserializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
        {
            // Check type

            if ((byte)Type != reader.ReadByte())
                throw new FormatException();

            // Read version

            Version = reader.ReadByte();

            // Deserialize exclusive data

            DeserializeExclusiveData(deserializer, reader, settings);

            // Deserialize shared content

            Attributes = deserializer.Deserialize<TransactionAttribute[]>(reader, settings);
            Inputs = deserializer.Deserialize<CoinReference[]>(reader, settings);
            Outputs = deserializer.Deserialize<TransactionOutput[]>(reader, settings);

            if (Outputs.Length > ushort.MaxValue) throw new FormatException();

            // Deserialize signature

            if (settings == null || settings.Filter == null || settings.Filter.Invoke(SignatureOrder))
            {
                Scripts = deserializer.Deserialize<Witness[]>(reader, settings);
                if (Scripts.Length > ushort.MaxValue) throw new FormatException();
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

            if (settings == null || settings.Filter == null || settings.Filter.Invoke(SignatureOrder))
            {
                ret += serializer.Serialize(Scripts, writer, settings);
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
        protected virtual void DeserializeExclusiveData(IBinaryDeserializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
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

        /// <summary>
        /// Get hash data
        /// </summary>
        /// <returns>Return hash data</returns>
        public override byte[] GetHashData(IBinarySerializer serializer)
        {
            // Exclude signature

            return serializer.Serialize(this, new BinarySerializerSettings()
            {
                Filter = (order => order != SignatureOrder)
            });
        }

        /// <summary>
        /// Verify
        /// </summary>
        /// <returns></returns>
        public virtual bool Verify()
        {
            return true;
        }
    }
}