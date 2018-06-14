using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class IssueTransaction : Transaction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public IssueTransaction() : base(TransactionType.IssueTransaction) { }
    }
}