using System.IO;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class StateTransaction : Transaction
    {
        /// <summary>
        /// Descriptors
        /// </summary>
        public StateDescriptor[] Descriptors;

        /// <inheritdoc />
        public StateTransaction() : base(TransactionType.StateTransaction) { }

        #region Exclusive data

        protected override void DeserializeExclusiveData(IBinarySerializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
        {
            Descriptors = deserializer.Deserialize<StateDescriptor[]>(reader, settings);
        }

        protected override int SerializeExclusiveData(IBinarySerializer serializer, BinaryWriter writer, BinarySerializerSettings settings = null)
        {
            return serializer.Serialize(Descriptors, writer, settings);
        }

        #endregion

        //public override bool Verify()
        //{
        //    if (Descriptors == null || Descriptors.Length > 16)
        //    {
        //        throw new FormatException(nameof(Descriptors));
        //    }

        //    foreach (StateDescriptor descriptor in Descriptors)
        //        if (!descriptor.Verify())
        //            return false;

        //    return base.Verify();
        //}
    }
}