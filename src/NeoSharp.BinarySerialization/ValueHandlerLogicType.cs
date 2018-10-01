namespace NeoSharp.BinarySerialization
{
    public enum ValueHandlerLogicType
    {
        /// <summary>
        /// Is writable
        /// </summary>
        Writable,

        /// <summary>
        /// Readonly, but don't care about the content
        /// </summary>
        JustConsume,

        /// <summary>
        /// ReadOnly, but must be equal as default value
        /// </summary>
        MustBeEqual
    }
}