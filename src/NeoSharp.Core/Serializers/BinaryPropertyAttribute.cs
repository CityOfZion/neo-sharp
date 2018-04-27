using System;

namespace NeoSharp.Core.Serializers
{
    public class BinaryPropertyAttribute : Attribute
    {
        /// <summary>
        /// Order
        /// </summary>
        public int Order { get; }

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