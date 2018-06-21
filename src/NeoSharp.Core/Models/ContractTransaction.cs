using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class ContractTransaction : Transaction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ContractTransaction() : base(TransactionType.ContractTransaction) { }

        /// <summary>
        /// Verify
        /// </summary>
        /// <returns>Return true if is verified</returns>
        public override bool Verify()
        {
            if (Version != 0x00) return false;

            return base.Verify();
        }
    }
}