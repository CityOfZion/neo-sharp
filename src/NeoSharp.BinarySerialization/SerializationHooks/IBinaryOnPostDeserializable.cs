namespace NeoSharp.BinarySerialization.SerializationHooks
{
    public interface IBinaryOnPostDeserializable
    {
        /// <summary>
        /// After on deserialize process
        /// </summary>
        void OnPostDeserialize();
    }
}