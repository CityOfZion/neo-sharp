using System.IO;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class ClaimTransaction : Transaction
    {
        /// <summary>
        /// Claims
        /// </summary>
        [BinaryProperty(100)]
        public CoinReference[] Claims;

        /// <inheritdoc />
        public ClaimTransaction() : base(TransactionType.ClaimTransaction) { }

        #region Exclusive serialization

        protected override void DeserializeExclusiveData(IBinarySerializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
        {
            Claims = deserializer.Deserialize<CoinReference[]>(reader, settings);
        }

        protected override int SerializeExclusiveData(IBinarySerializer serializer, BinaryWriter writer, BinarySerializerSettings settings = null)
        {
            return serializer.Serialize(Claims, writer, settings);
        }

        #endregion
    }
}