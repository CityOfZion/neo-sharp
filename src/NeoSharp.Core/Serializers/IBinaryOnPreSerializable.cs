namespace NeoSharp.Core.Serializers
{
    public interface IBinaryOnPreSerializable
    {
        /// <summary>
        /// Before on serialize process
        /// </summary>
        void OnPreSerialize();
    }
}