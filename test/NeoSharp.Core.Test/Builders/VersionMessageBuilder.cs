using System;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Messaging.Messages;

namespace NeoSharp.Core.Test.Builders
{
    public class VersionMessageBuilder
    {
        private int _length;

        public VersionMessageBuilder WithLength(int length)
        {
            this._length = length;
            return this;
        }

        public VersionMessage Build()
        {
            var r = new Random(Environment.TickCount);
            var versionPayload = new VersionPayload
            {
                Version = (uint)r.Next(0, int.MaxValue),
                Services = (ulong)r.Next(0, int.MaxValue),
                Timestamp = DateTime.UtcNow.ToTimestamp(),
                Port = (ushort)r.Next(0, short.MaxValue),
                Nonce = (uint)r.Next(0, int.MaxValue),
                UserAgent = $"/NEO:{r.Next(1, 10)}.{r.Next(1, 100)}.{r.Next(1, 1000)}/" + "0".PadLeft(this._length, '0'),
                CurrentBlockIndex = (uint)r.Next(0, int.MaxValue),
                Relay = false
            };

            return new VersionMessage(versionPayload);
        }
    }
}
