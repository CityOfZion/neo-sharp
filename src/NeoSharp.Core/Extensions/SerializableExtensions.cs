using NeoSharp.Core.Types;
using System.IO;
using System.Text;

namespace NeoSharp.Core.Extensions
{
    public static class SerializableExtensions
    {
        public static byte[] ToArray(this ISerializable value)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms, Encoding.UTF8))
            {
                value.Serialize(writer);
                writer.Flush();
                return ms.ToArray();
            }
        }
    }
}