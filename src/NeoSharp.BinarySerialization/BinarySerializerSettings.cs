using System;

namespace NeoSharp.BinarySerialization
{
    public class BinarySerializerSettings
    {
        /// <summary>
        /// Filter
        /// </summary>
        public Predicate<string> Filter { get; set; }
    }
}