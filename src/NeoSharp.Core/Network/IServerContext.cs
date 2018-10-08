using System.Collections.Concurrent;
using System.Collections.Generic;
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
        /// Connected peers
        /// </summary>
        ConcurrentDictionary<EndPoint, IPeer> ConnectedPeers { get; }
    }
}