using System;
using System.Threading.Tasks;
using NeoSharp.Core.Messaging;

namespace NeoSharp.Core.Network
{
    public interface IBroadcast
    {
        /// <summary>
        /// Broadcast a message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="filter">Filter</param>
        Task SendBroadcast(Message message, Func<IPeer, bool> filter = null);
    }
}