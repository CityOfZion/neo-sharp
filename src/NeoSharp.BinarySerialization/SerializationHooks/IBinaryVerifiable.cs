namespace NeoSharp.BinarySerialization.SerializationHooks
{
    public interface IBinaryVerifiable
    {
        /// <summary>
        /// Verify after deserialization
        /// </summary>
        /// <returns></returns>
        bool Verify();
    }
}