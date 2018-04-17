namespace NeoSharp.Core.Network
{
    public class EndPoint
    {
        public Protocol Protocol { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public override string ToString()
        {
            return $"{Protocol}://{Host}:{Port}".ToLowerInvariant();
        }
    }

    public enum Protocol
    {
        Unknown = 0,
        Tcp = 1,
        // Tls = 2,
        Http = 3,
        // Https = 4,
        Ws = 5,
        // Wss = 6
    }
}