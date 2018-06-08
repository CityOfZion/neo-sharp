using NeoSharp.Core.Messaging.Messages;

namespace NeoSharp.Core.Network
{
    public interface IServerContext
    {
        /// <summary>
        /// Version
        /// </summary>
        VersionPayload Version { get; }

        /// <summary>
        /// Update version paylload
        /// </summary>
        /// <param name="port">Port</param>
        /// <param name="blockchainCurrentBlockIndex">Blockchain current block index</param>
        void BuiltVersionPayload(ushort port, uint blockchainCurrentBlockIndex);

        /// <summary>
        /// Update version paylload
        /// </summary>
        /// <param name="blockchainCurrentBlockIndex">Blockchain current block index</param>
        void BuiltVersionPayload(uint blockchainCurrentBlockIndex);
    }
}
