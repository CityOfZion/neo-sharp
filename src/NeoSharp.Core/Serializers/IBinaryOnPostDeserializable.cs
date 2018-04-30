namespace NeoSharp.Core.Serializers
{
    public interface IBinaryOnPostDeserializable
    {
        /// <summary>
        /// After on deserialize process
        /// </summary>
        void OnPostDeserialize();
    }
}