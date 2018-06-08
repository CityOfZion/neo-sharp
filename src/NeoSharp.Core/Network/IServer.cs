using NeoSharp.Core.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeoSharp.Core.Network
{
    public interface IServer
    {
        /// <summary>
        /// Connected peers
        /// </summary>
        IReadOnlyCollection<IPeer> ConnectedPeers { get; }

        /// <summary>
        /// Start server
        /// </summary>
        void Start();

        /// <summary>
        /// Stop server
        /// </summary>
        void Stop();

        /// <summary>
        /// Broadcast a message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="filter">Filter</param>
        Task SendBroadcast(Message message, Func<IPeer, bool> filter = null);
    }
}