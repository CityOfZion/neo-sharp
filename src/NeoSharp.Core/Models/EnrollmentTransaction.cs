using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionTypeSerializer))]
    public class EnrollmentTransaction : Transaction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EnrollmentTransaction() : base(TransactionType.EnrollmentTransaction) { }
    }
}