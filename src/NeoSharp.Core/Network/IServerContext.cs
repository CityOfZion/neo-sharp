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
        /// Update version payload
        /// </summary>
        void UpdateVersionPayload();
    }
}
