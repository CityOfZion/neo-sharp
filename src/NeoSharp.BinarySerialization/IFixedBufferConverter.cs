namespace NeoSharp.BinarySerialization
{
    public interface IFixedBufferConverter
    {
        /// <summary>
        /// Fixed length of the buffer
        /// </summary>
        int FixedLength { get; }
    }
}