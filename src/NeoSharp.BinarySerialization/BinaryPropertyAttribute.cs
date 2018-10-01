using System;

namespace NeoSharp.BinarySerialization
{
    public class BinaryPropertyAttribute : Attribute
    {
        /// <summary>
        /// Order
        /// </summary>
        public readonly int Order;

        /// <summary>
        /// Override one property that use the same Order number
        /// </summary>
        public bool Override { get; set; } = false;

        /// <summary>
        /// Max length (used for strings and arrays)
        /// </summary>
        public int MaxLength { get; set; } = ushort.MaxValue;

        /// <summary>
        /// Value handler logic
        /// </summary>
        public ValueHandlerLogicType ValueHandlerLogic { get; set; } = ValueHandlerLogicType.Writable;

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