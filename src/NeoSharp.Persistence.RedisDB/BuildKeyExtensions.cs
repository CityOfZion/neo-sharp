using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;

namespace NeoSharp.Persistence.RedisDB
{
    public static class BuildKeyExtensions
    {
        public static string BuildDataBlockKey(this UInt256 hash)
        {
            return DataEntryPrefix.DataBlock.BuildKey(hash.ToString());
        }

        public static string BuildDataTransactionKey(this UInt256 hash)
        {
            return DataEntryPrefix.DataTransaction.BuildKey(hash.ToString());
        }

        public static string BuildIxHeightToHashKey(this uint index)
        {
            return DataEntryPrefix.IxHeightToHash.BuildKey(index.ToString());
        }

        public static string BuildIxConfirmedKey(this UInt160 hash)
        {
            return DataEntryPrefix.IxConfirmed.BuildKey(hash.ToString());
        }

        public static string BuildIxClaimableKey(this UInt160 hash)
        {
            return DataEntryPrefix.IxClaimable.BuildKey(hash.ToString());
        }

        private static string BuildKey(this DataEntryPrefix type, string key)
        {
            return string.Format("{0}:{1}", key, type);
        }
    }
}