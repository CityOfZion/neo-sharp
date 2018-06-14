using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class InvocationTransaction : Transaction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InvocationTransaction() : base(TransactionType.InvocationTransaction) { }
    }
}