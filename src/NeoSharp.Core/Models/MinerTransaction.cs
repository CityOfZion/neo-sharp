using System;
using System.IO;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;
using NeoSharp.Core.Types;

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
        protected override void DeserializeExclusiveData(IBinaryDeserializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
        {
            Nonce = reader.ReadUInt32();
        }

        protected override int SerializeExclusiveData(IBinarySerializer serializer, BinaryWriter writer, BinarySerializerSettings settings = null)
        {
            writer.Write(Nonce);
            return 4;
        }
        #endregion

        //public override bool Verify()
        //{
        //    if (Version != 0) throw new FormatException(nameof(Version));

        //    if (Inputs.Length != 0)
        //        throw new FormatException(nameof(Inputs));

        //    if (Outputs.Any(p => p.AssetId != TransactionContext.UtilityTokenHash))
        //        throw new FormatException(nameof(Outputs));

        //    return base.Verify();
        //}
    }
}