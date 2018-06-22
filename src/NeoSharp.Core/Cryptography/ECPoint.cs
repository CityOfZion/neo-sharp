using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.Cryptography
{
    [BinaryTypeSerializer(typeof(ECPointBinarySerializer))]
    public class ECPoint
    {
        public readonly static ECPoint Infinity = new ECPoint();

        /// <summary>
        /// Data
        /// </summary>
        public readonly byte[] Data;
        /// <summary>
        /// Is infinite
        /// </summary>
        public readonly bool IsInfinity;

        /// <summary>
        /// Infinite constructor
        /// </summary>
        public ECPoint()
        {
            IsInfinity = true;
            Data = new byte[] { 0x00 };
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Data</param>
        public ECPoint(byte[] data)
        {
            IsInfinity = false;
            Data = data;
        }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return Data.ToHexString();
        }
        /// <summary>
        /// String representation
        /// </summary>
        /// <param name="append0x">Append 0x</param>
        public string ToString(bool append0x)
        {
            return Data.ToHexString(append0x);
        }
    }
}