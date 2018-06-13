using System;
using NeoSharp.BinarySerialization;
using NeoSharp.BinarySerialization.SerializationHooks;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionTypeSerializer))]
    public class ClaimTransaction : Transaction, IBinaryVerifiable
    {
        /// <summary>
        /// Claims
        /// </summary>
        public CoinReference[] Claims;

        /// <summary>
        /// Constructor
        /// </summary>
        public ClaimTransaction() : base(TransactionType.ClaimTransaction) { }

        /// <summary>
        /// Verify
        /// </summary>
        public bool Verify()
        {
            if (Version != 0) throw new ArgumentException(nameof(Version));
            if (Claims == null || Claims.Length == 0) throw new ArgumentException(nameof(Claims));

            return true;
        }
    }
}