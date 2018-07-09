using System;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;

namespace NeoSharp.Persistence.RocksDB
{
    public static class BuildKeyExtensions
    {
        public static byte[] BuildDataBlockKey(this UInt256 hash)
        {
            return hash.ToArray().BuildKey(DataEntryPrefix.DataBlock);
        }

        public static byte[] BuildDataTransactionKey(this UInt256 hash)
        {
            return hash.ToArray().BuildKey(DataEntryPrefix.DataTransaction);
        }

        public static byte[] BuildIxHeightToHashKey(this uint index)
        {
            var indexByteArray = BitConverter.GetBytes(index);

            return indexByteArray.BuildKey(DataEntryPrefix.IxHeightToHash);
        }

        private static byte[] BuildKey(this byte[] key, DataEntryPrefix dataEntryPrefix)
        {
            var len = key.Length;
            var bytes = new byte[len + 1];

            bytes[0] = (byte)dataEntryPrefix;
            Array.Copy(key, 0, bytes, 1, len);

            return bytes;
        }
    }
}
