using System.Collections.Generic;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Persistence
{
    public static class ByteArrayExtensionMethods
    {
        public static byte[] BuildKey(this byte[] hash, DataEntryPrefix dataEntryPrefix)
        {
            var bytes = new List<byte>(hash);
            bytes.Insert(0, (byte) dataEntryPrefix);
            return bytes.ToArray();
        }

        public static byte[] BuildKey(this UInt256 hash, DataEntryPrefix dataEntryPrefix)
        {
            var bytes = new List<byte>(hash.ToArray());
            bytes.Insert(0, (byte) dataEntryPrefix);
            return bytes.ToArray();
        }
    }
}
