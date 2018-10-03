using System;
using System.Linq;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Types;

namespace NeoSharp.Persistence.RocksDB
{
    public static class BuildKeyExtensions
    {
        public static byte[] BuildDataBlockKey(this UInt256 hash)
        {
            return DataEntryPrefix.DataBlock.BuildKey(hash.ToArray());
        }

        public static byte[] BuildDataTransactionKey(this UInt256 hash)
        {
            return DataEntryPrefix.DataTransaction.BuildKey(hash.ToArray());
        }

        public static byte[] BuildStateAccountKey(this UInt160 hash)
        {
            return DataEntryPrefix.StAccount.BuildKey(hash.ToArray());
        }

        public static byte[] BuildStateCoinKey(this UInt256 hash)
        {
            return DataEntryPrefix.StCoin.BuildKey(hash.ToArray());
        }

        public static byte[] BuildStateValidatorKey(this ECPoint point)
        {
            return DataEntryPrefix.StValidator.BuildKey(point.EncodedData);
        }

        public static byte[] BuildStateAssetKey(this UInt256 hash)
        {
            return DataEntryPrefix.StAsset.BuildKey(hash.ToArray());
        }

        public static byte[] BuildStateContractKey(this UInt160 hash)
        {
            return DataEntryPrefix.StContract.BuildKey(hash.ToArray());
        }

        public static byte[] BuildStateStorageKey(this StorageKey key)
        {
            return DataEntryPrefix.StStorage.BuildKey(key.ScriptHash.ToArray().Concat(key.Key).ToArray());
        }

        public static byte[] BuildIndexConfirmedKey(this UInt160 hash)
        {
            return DataEntryPrefix.IxConfirmed.BuildKey(hash.ToArray());
        }

        public static byte[] BuildIndexClaimableKey(this UInt160 hash)
        {
            return DataEntryPrefix.IxClaimable.BuildKey(hash.ToArray());
        }

        public static byte[] BuildIxHeightToHashKey(this uint index)
        {
            var indexByteArray = BitConverter.GetBytes(index);

            return DataEntryPrefix.IxHeightToHash.BuildKey(indexByteArray);
        }

        private static byte[] BuildKey(this DataEntryPrefix dataEntryPrefix, byte[] key)
        {
            var len = key.Length;
            var bytes = new byte[len + 1];

            bytes[0] = (byte)dataEntryPrefix;
            Array.Copy(key, 0, bytes, 1, len);

            return bytes;
        }
    }
}