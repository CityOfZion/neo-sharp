using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionTypeSerializer))]
    public class StateTransaction : Transaction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public StateTransaction() : base(TransactionType.StateTransaction) { }
    }
}