using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class RegisterTransaction : Transaction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RegisterTransaction() : base(TransactionType.RegisterTransaction) { }
    }
}