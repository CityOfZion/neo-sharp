using System;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;
using NeoSharp.VM;

namespace NeoSharp.Core.SmartContract
{
    public class MessageContainer : IMessageContainer
    {
        private readonly IBinarySerializer _binarySerializer;

        private object _message;
        private byte[] _messageData;

        public MessageContainer(IBinarySerializer binarySerializer)
        {
            _binarySerializer = binarySerializer;
        }

        public object GetMessage(uint iteration) => _message;

        public byte[] GetMessageData(uint iteration) => _messageData ?? Array.Empty<byte>();

        public void RegisterMessage(object message)
        {
            _message = message ?? throw new ArgumentNullException(nameof(message));
            _messageData = _binarySerializer.Serialize(message);
        }
    }
}