namespace NeoSharp.VM
{
    public abstract class IStackItem<T> : IStackItem
    {
        /// <summary>
        /// Value
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="type">Type</param>
        protected IStackItem(T data, EStackItemType type) : base(type)
        {
            Value = data;
        }

        /// <summary>
        /// Get raw object
        /// </summary>
        /// <returns>Raw object</returns>
        public override object GetRawObject()
        {
            return Value;
        }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return Value?.ToString();
        }
    }
}