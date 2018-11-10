using System;
using System.Linq;
using NeoSharp.VM;

namespace NeoSharp.Core.SmartContract
{
    public class MessageContainer : IMessageContainer
    {
        private byte[] _message = Array.Empty<byte>();

        public byte[] GetMessage(uint iteration)
        {
            return _message;
        }

        public void RegisterMessage(byte[] message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (message.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(message));

            _message = message.ToArray();
        }
    }
}