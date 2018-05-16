namespace NeoSharp.Core.Network
{
    public interface IRpcProcessRequest
    {
        /// <summary>
        /// Process request
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Return object or null</returns>
        object Process(RpcRequest request);
    }
}