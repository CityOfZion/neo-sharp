using System.Collections.Generic;

namespace NeoSharp.Core.Persistence
{
    public static class ByteArrayExtensionMethods
    {
        public static byte[] BuildKey(this byte[] hash, DataEntryPrefix dataEntryPrefix)
        {
            var bytes = new List<byte>(hash);
            bytes.Insert(0, (byte)dataEntryPrefix);
            return bytes.ToArray();
        }
    }
}
