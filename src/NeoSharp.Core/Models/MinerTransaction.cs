using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionTypeSerializer))]
    public class MinerTransaction : Transaction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MinerTransaction() : base(TransactionType.MinerTransaction) { }
    }
}