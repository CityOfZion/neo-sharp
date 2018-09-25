using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class IssueTransaction : Transaction
    {
        /// <inheritdoc />
        public IssueTransaction() : base(TransactionType.IssueTransaction) { }
    }
}