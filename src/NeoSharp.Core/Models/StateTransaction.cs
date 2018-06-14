using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class StateTransaction : Transaction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public StateTransaction() : base(TransactionType.StateTransaction) { }
    }
}