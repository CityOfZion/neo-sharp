using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using NeoSharp.Core.DI;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging
{
    public class MessageHandlerProxy : IMessageHandler<Message>
    {
        private readonly IContainer _container;
        private readonly ILogger<MessageHandlerProxy> _logger;
        private readonly IReadOnlyDictionary<Type, Delegate> _messageHandlerInvokers;

        public MessageHandlerProxy(IContainer container, IEnumerable<Type> messageHandlerTypes, ILogger<MessageHandlerProxy> logger)
        {
            _container = container;
            _logger = logger;
            _messageHandlerInvokers = messageHandlerTypes
                .Select(CreateMessageHandlerInvoker)
                .ToDictionary(x => x.MessageType, x => x.MessageHandlerInvoker);
        }

        public Task Handle(Message message, IPeer sender)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();
            var messageHandler = ResolveMessageHandler(messageType);
            var startedAt = LogMessageHandlingStart(messageHandler);
            var messageHandlerInvoker = GetMessageHandlerInvoker(messageType);
            var handleMessageTask = (Task)messageHandlerInvoker.DynamicInvoke(messageHandler, message, sender);

            return handleMessageTask.ContinueWith(t =>
            {
                if (!t.IsCompletedSuccessfully) return;

                LogMessageHandlingEnd(startedAt, messageHandler);
            });
        }

        private static (Type MessageType, Delegate MessageHandlerInvoker) CreateMessageHandlerInvoker(Type messageHandlerType)
        {
            const string handleMethodName = nameof(IMessageHandler<Message>.Handle);
            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;

            var handleMethodInfo = messageHandlerType.GetMethod(handleMethodName, bindingFlags);
            var messageType = handleMethodInfo.GetParameters().First().ParameterType;
            var messageHandlerInvoker = Delegate.CreateDelegate(
                Expression.GetFuncType(messageHandlerType, messageType, typeof(IPeer), typeof(Task)),
                null,
                handleMethodInfo);

            return (messageType, messageHandlerInvoker);
        }

        private object ResolveMessageHandler(Type messageType)
        {
            var messageHandler = _container.Resolve(typeof(IMessageHandler<>).MakeGenericType(messageType));
            if (messageHandler == null)
            {
                throw new InvalidOperationException(
                    $"The message of \"{messageType}\" type has no registered handlers.");
            }

            return messageHandler;
        }

        private Delegate GetMessageHandlerInvoker(Type messageType)
        {
            if (_messageHandlerInvokers.TryGetValue(messageType, out var messageHandlerInvoker) == false)
            {
                throw new InvalidOperationException(
                    $"The message of \"{messageType}\" type has no registered handlers.");
            }

            return messageHandlerInvoker;
        }

        private DateTime LogMessageHandlingStart(object messageHandler)
        {
            var startedAt = DateTime.Now;
            var messageHandlerName = messageHandler.GetType().Name;

            _logger.LogDebug(
                $"The message handler \"{messageHandlerName}\" started message handling at {startedAt:yyyy-MM-dd HH:mm:ss}.");

            return startedAt;
        }

        private void LogMessageHandlingEnd(DateTime startedAt, object messageHandler)
        {
            var completedAt = DateTime.Now;
            var handledWithin = (completedAt - startedAt).TotalMilliseconds;
            var messageHandlerName = messageHandler.GetType().Name;

            _logger.LogDebug(
                $"The message handler \"{messageHandlerName}\" completed message handling at {completedAt:yyyy-MM-dd HH:mm:ss} ({handledWithin} ms).");
        }
    }
}