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
    }
}