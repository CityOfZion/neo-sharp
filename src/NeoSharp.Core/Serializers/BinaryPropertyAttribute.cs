using System;

namespace NeoSharp.Core.Serializers
{
    public class BinaryPropertyAttribute : Attribute
    {
        /// <summary>
        /// Order
        /// </summary>
        public readonly int Order;
        /// <summary>
        /// Max length (used for strings and arrays)
        /// </summary>
        public int MaxLength { get; set; } = ushort.MaxValue;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="order">Order</param>
        public BinaryPropertyAttribute(int order)
        {
            Order = order;
        }
    }
}