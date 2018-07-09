using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;

namespace NeoSharp.Persistence.RedisDB
{
    public static class BuildKeyExtensions
    {
        public static string BuildDataBlockKey(this UInt256 hash)
        {
            return BuildKey(DataEntryPrefix.DataBlock, hash.ToString());
        }

        public static string BuildDataTransactionKey(this UInt256 hash)
        {
            return BuildKey(DataEntryPrefix.DataTransaction, hash.ToString());
        }

        public static string BuildIxHeightToHashKey(this uint index)
        {
            return BuildKey(DataEntryPrefix.IxHeightToHash, index.ToString());
        }

        private static string BuildKey(DataEntryPrefix type, string key)
        {
            return string.Format("{0}:{1}", key, type);
        }
    }
}
