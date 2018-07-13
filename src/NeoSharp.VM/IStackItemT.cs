namespace NeoSharp.VM
{
    public abstract class IStackItem<T> : IStackItem
    {
        /// <summary>
        /// Value
        /// </summary>
        public readonly T Value;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="data">Data</param>
        /// <param name="type">Type</param>
        protected IStackItem(IExecutionEngine engine, T data, EStackItemType type) : base(engine, type)
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