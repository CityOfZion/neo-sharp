using System;
using System.IO;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;
using NeoSharp.Core.Cryptography;

namespace NeoSharp.Core.Models
{
    [Obsolete]
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class EnrollmentTransaction : Transaction
    {
        /// <summary>
        /// PublicKey
        /// </summary>
        public ECPoint PublicKey;

        /// <summary>
        /// Constructor
        /// </summary>
        public EnrollmentTransaction() : base(TransactionType.EnrollmentTransaction) { }

        protected override void DeserializeExclusiveData(IBinarySerializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
        {
            PublicKey = deserializer.Deserialize<ECPoint>(reader, settings);
        }

        protected override int SerializeExclusiveData(IBinarySerializer serializer, BinaryWriter writer, BinarySerializerSettings settings = null)
        {
            return serializer.Serialize(PublicKey, writer, settings);
        }
    }
}