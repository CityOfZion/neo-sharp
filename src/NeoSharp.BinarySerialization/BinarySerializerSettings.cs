using System;

namespace NeoSharp.BinarySerialization
{
    public class BinarySerializerSettings
    {
        /// <summary>
        /// Filter by order
        /// </summary>
        public Func<int, bool> Filter { get; set; }
    }
}