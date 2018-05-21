namespace NeoSharp.Core.Network.Rpc
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