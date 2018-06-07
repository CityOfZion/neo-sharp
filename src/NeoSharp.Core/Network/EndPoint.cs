namespace NeoSharp.Core.Network
{
    public class EndPoint
    {
        /// <summary>
        /// Protocol
        /// </summary>
        public Protocol Protocol { get; set; }
        /// <summary>
        /// Host
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return $"{Protocol.ToString().ToLowerInvariant()}://{Host}:{Port}";
        }
    }

    public enum Protocol
    {
        Unknown = 0,
        Tcp = 1,
        // Tls = 2,
        Ws = 3,
        // Wss = 4
    }
}