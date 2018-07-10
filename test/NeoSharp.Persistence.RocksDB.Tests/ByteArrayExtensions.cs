using System.Linq;

namespace NeoSharp.Persistence.RocksDB.Tests
{
    public static class ByteArrayExtensions
    {
        public static bool ArrayIsEquivalentTo(this byte[] source, byte[] target)
        {
            return source.Length == target.Length && source.SequenceEqual(target);
        }
    }
}