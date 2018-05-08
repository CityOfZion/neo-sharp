namespace NeoSharp.BinarySerialization.Interfaces
{
    public interface IBinaryOnPreSerializable
    {
        /// <summary>
        /// Before on serialize process
        /// </summary>
        void OnPreSerialize();
    }
}