using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging
{
    public class MessageHandlerProxy : IMessageHandlerProxy
    {
        #region Private fields 
        private readonly IEnumerable<IMessageHandler> _messageHandlers;
        private readonly ILogger<MessageHandlerProxy> _logger;

        //private readonly Dictionary<Type, IMessageHandler> _handlers;
        #endregion

        #region Constructor 
        public MessageHandlerProxy(
            IEnumerable<IMessageHandler> messageHandlers, 
            ILogger<MessageHandlerProxy> logger)
        {
            _messageHandlers = messageHandlers;
            _logger = logger;
        }
        #endregion

        #region MessageHandler implementation 
        public async Task Handle(Message message, IPeer sender)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var messageHandler = this._messageHandlers.SingleOrDefault(x => x.CanHandle(message));

            if (messageHandler == null)
            {
                this._logger.LogError($"could not find the handler for the message type {message.GetType()}");
                return;
            }

            var startedAt = DateTime.UtcNow;
            _logger.LogDebug($"The message handler \"{messageHandler.GetType().Name}\" started message handling at {startedAt:yyyy-MM-dd HH:mm:ss}.");

            dynamic specificMessageHandler = Convert.ChangeType(messageHandler, messageHandler.GetType());
            dynamic specificMessage = Convert.ChangeType(message, message.GetType());
            await specificMessageHandler.Handle(specificMessage, sender);
            var completedAt = DateTime.UtcNow;

            var handledWithin = (completedAt - startedAt).TotalSeconds;

            _logger.LogDebug(
                $"The message handler \"{messageHandler.GetType().Name}\" completed message handling at {completedAt:yyyy-MM-dd HH:mm:ss} ({handledWithin} s).");
        }
        #endregion
    }
}
