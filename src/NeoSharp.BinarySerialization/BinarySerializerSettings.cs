using System;

namespace NeoSharp.BinarySerialization
{
    public class BinarySerializerSettings
    {
        /// <summary>
        /// Filter
        /// </summary>
        public Func<BinaryPropertyAttribute, bool> Filter { get; set; }
    }
}