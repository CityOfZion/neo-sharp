using System.IO;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class MinerTransaction : Transaction
    {
        /// <summary>
        /// Random number
        /// </summary>
        public uint Nonce;

        /// <inheritdoc />
        public MinerTransaction() : base(TransactionType.MinerTransaction) { }

        #region Exclusive serialization
        protected override void DeserializeExclusiveData(IBinarySerializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
        {
            Nonce = reader.ReadUInt32();
        }

        protected override int SerializeExclusiveData(IBinarySerializer serializer, BinaryWriter writer, BinarySerializerSettings settings = null)
        {
            writer.Write(Nonce);
            return 4;
        }
        #endregion
    }
}