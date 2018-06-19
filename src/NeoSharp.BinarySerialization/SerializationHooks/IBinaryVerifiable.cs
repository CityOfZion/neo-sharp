namespace NeoSharp.BinarySerialization.SerializationHooks
{
    public interface IBinaryVerifiable
    {
        /// <summary>
        /// Verify after deserialization
        /// </summary>
        /// <returns>Return true if is verified</returns>
        bool Verify();
    }
}