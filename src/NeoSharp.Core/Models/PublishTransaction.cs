using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionTypeSerializer))]
    public class PublishTransaction : Transaction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PublishTransaction() : base(TransactionType.PublishTransaction) { }
    }
}