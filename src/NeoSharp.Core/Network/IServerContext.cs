using NeoSharp.Core.Messaging.Messages;

namespace NeoSharp.Core.Network
{
    public interface IServerContext
    {
        VersionPayload Version { get; }

        void BuiltVersionPayload(ushort port, uint blockchainCurrentBlockIndex);
    }
}
