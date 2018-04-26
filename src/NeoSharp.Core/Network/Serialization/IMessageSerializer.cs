using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Network.Messages;

namespace NeoSharp.Core.Network.Serialization
{
    public interface IMessageSerializer
    {
        Task SerializeTo<TMessage>(TMessage message, Stream stream, CancellationToken cancellationToken)
            where TMessage : Message;


        Task<TMessage> DeserializeFrom<TMessage>(Stream stream, CancellationToken cancellationToken)
            where TMessage : Message, new();
    }
}