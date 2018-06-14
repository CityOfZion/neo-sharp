using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class PublishTransaction : Transaction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PublishTransaction() : base(TransactionType.PublishTransaction) { }
    }
}