using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class ContractTransaction : Transaction
    {
        /// <inheritdoc />
        public ContractTransaction() : base(TransactionType.ContractTransaction) { }
    }
}