namespace NeoSharp.BinarySerialization.Interfaces
{
    public interface IBinaryOnPostDeserializable
    {
        /// <summary>
        /// After on deserialize process
        /// </summary>
        void OnPostDeserialize();
    }
}