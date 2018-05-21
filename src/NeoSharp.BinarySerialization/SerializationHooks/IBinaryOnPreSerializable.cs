namespace NeoSharp.BinarySerialization.SerializationHooks
{
    public interface IBinaryOnPreSerializable
    {
        /// <summary>
        /// Before on serialize process
        /// </summary>
        void OnPreSerialize();
    }
}