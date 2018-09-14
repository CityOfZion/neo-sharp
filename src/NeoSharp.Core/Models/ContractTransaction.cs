using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class ContractTransaction : Transaction
    {
        /// <inheritdoc />
        public ContractTransaction() : base(TransactionType.ContractTransaction) { }


        //public override bool Verify()
        //{
        //    if (Version != 0x00) return false;

        //    return base.Verify();
        //}
    }
}