using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class IssueTransaction : Transaction
    {
        /// <inheritdoc />
        public IssueTransaction() : base(TransactionType.IssueTransaction) { }

        //public override bool Verify()
        //{
        //    if (Version > 1) throw new FormatException(nameof(Version));

        //    return base.Verify();
        //}
    }
}