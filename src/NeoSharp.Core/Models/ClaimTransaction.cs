using System;
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

        /// <summary>
        /// Constructor
        /// </summary>
        public ClaimTransaction() : base(TransactionType.ClaimTransaction) { }

        protected override void DeserializeExclusiveData(IBinaryDeserializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
        {
            Claims = deserializer.Deserialize<CoinReference[]>(reader, settings);
        }

        protected override int SerializeExclusiveData(IBinarySerializer serializer, BinaryWriter writer, BinarySerializerSettings settings = null)
        {
            return serializer.Serialize(Claims, writer, settings);
        }

        /// <summary>
        /// Verify
        /// </summary>
        public override bool Verify()
        {
            if (Version != 0) throw new ArgumentException(nameof(Version));
            if (Claims == null || Claims.Length == 0) throw new ArgumentException(nameof(Claims));

            return base.Verify();
        }
    }
}