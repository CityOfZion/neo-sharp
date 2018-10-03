using System.Linq;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Types;

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

        public static string BuildStateAccountKey(this UInt160 hash)
        {
            return DataEntryPrefix.StAccount.BuildKey(hash.ToString());
        }

        public static string BuildStateCoinKey(this UInt256 hash)
        {
            return DataEntryPrefix.StCoin.BuildKey(hash.ToString());
        }

        public static string BuildStateAssetKey(this UInt256 hash)
        {
            return DataEntryPrefix.StAsset.BuildKey(hash.ToString());
        }

        public static string BuildStateContractKey(this UInt160 hash)
        {
            return DataEntryPrefix.StContract.BuildKey(hash.ToString());
        }

        public static string BuildStateStorageKey(this StorageKey key)
        {
            return DataEntryPrefix.StStorage.BuildKey(key.ScriptHash.ToArray().Concat(key.Key).ToString());
        }

        public static string BuildStateValidatorKey(this ECPoint point)
        {
            return DataEntryPrefix.StValidator.BuildKey(point.EncodedData.ToString());
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