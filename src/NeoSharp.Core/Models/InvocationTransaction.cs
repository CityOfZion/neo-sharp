using System;
using System.IO;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;
using NeoSharp.Types;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class InvocationTransaction : Transaction
    {
        /// <summary>
        /// Script
        /// </summary>
        public byte[] Script;
        /// <summary>
        /// Gas
        /// </summary>
        public Fixed8 Gas;

        /// <inheritdoc />
        public InvocationTransaction() : base(TransactionType.InvocationTransaction) { }

        /// <summary>
        /// Get Gas
        /// </summary>
        /// <param name="consumed">Consumed</param>
        /// <returns>Gas</returns>
        public static Fixed8 GetGas(Fixed8 consumed)
        {
            var gas = consumed - Fixed8.FromDecimal(10);

            return gas <= Fixed8.Zero ? 
                Fixed8.Zero : 
                gas.Ceiling();
        }

        #region Exclusive serialization

        protected override void DeserializeExclusiveData(IBinarySerializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
        {
            if (Version > 1) throw new FormatException(nameof(Version));

            Script = reader.ReadVarBytes(65536);

            if (Script.Length == 0) throw new FormatException();

            if (Version >= 1)
            {
                Gas = deserializer.Deserialize<Fixed8>(reader, settings);

                if (Gas < Fixed8.Zero) throw new FormatException();
            }
            else
            {
                Gas = Fixed8.Zero;
            }
        }

        protected override int SerializeExclusiveData(IBinarySerializer serializer, BinaryWriter writer, BinarySerializerSettings settings = null)
        {
            var l = writer.WriteVarBytes(Script);

            if (Version >= 1)
            {
                l += serializer.Serialize(Gas, writer, settings);
            }

            return l;
        }

        #endregion
    }
}