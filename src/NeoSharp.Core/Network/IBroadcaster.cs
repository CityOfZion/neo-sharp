using System.Threading.Tasks;
using NeoSharp.Core.Messaging;

namespace NeoSharp.Core.Network
{
    public interface IBroadcaster
    {
        /// <summary>
        /// Broadcast a message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="source">Source</param>
        void Broadcast(Message message, IPeer source = null);
    }
}