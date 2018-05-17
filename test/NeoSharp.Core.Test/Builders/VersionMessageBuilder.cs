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
            var message = new VersionMessage();
            var r = new Random(Environment.TickCount);

            message.Payload.Version = (uint)r.Next(0, int.MaxValue);
            message.Payload.Services = (ulong)r.Next(0, int.MaxValue);
            message.Payload.Timestamp = DateTime.UtcNow.ToTimestamp();
            message.Payload.Port = (ushort)r.Next(0, short.MaxValue);
            message.Payload.Nonce = (uint)r.Next(0, int.MaxValue);
            message.Payload.UserAgent = $"/NEO:{r.Next(1, 10)}.{r.Next(1, 100)}.{r.Next(1, 1000)}/" + ("0".PadLeft(this._length, '0'));
            message.Payload.CurrentBlockIndex = (uint)r.Next(0, int.MaxValue);
            message.Payload.Relay = false;

            return message;
        }
    }
}
