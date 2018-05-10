using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NeoSharp.Core.DI;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging
{
    public class MessageHandlerProxy : IMessageHandler<Message>
    {
        private readonly IContainer _container;
        private readonly ILogger<MessageHandlerProxy> _logger;
        private readonly IDictionary<Type, Delegate> _messageHandlerInvokers;
        private static readonly Type _messageHandlerInterfaceType = typeof(IMessageHandler<>);

        public MessageHandlerProxy(IContainer container, IReadOnlyCollection<Type> messageHandlerTypes, ILogger<MessageHandlerProxy> logger)
        {
            _container = container;
            _logger = logger;

            const string handleMethodName = nameof(IMessageHandler<Message>.Handle);
            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;

            var peerType = typeof(IPeer);
            var taskType = typeof(Task);

            _messageHandlerInvokers = new Dictionary<Type, Delegate>(messageHandlerTypes.Count);

            foreach (var messageHandlerType in messageHandlerTypes)
            {
                var handleMethodInfo = messageHandlerType
                    .GetMethod(handleMethodName, bindingFlags);

                var messageType = handleMethodInfo.GetParameters().First().ParameterType;

                var messageHandlerInvoker = Delegate.CreateDelegate(
                    Expression.GetFuncType(messageHandlerType, messageType, peerType, taskType),
                    null,
                    handleMethodInfo);

                _messageHandlerInvokers.Add(messageType, messageHandlerInvoker);
            }
        }

        public Task Handle(Message message, IPeer sender)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            if (_messageHandlerInvokers.TryGetValue(messageType, out var messageHandlerInvoker) == false)
            {
                throw new InvalidOperationException(
                    $"The message of \"{messageType}\" type has no registered handlers.");
            }

            var messageHandler = _container.Resolve(_messageHandlerInterfaceType.MakeGenericType(messageType));
            var startedAt = DateTime.Now;

            var messageHandlerName = messageHandler.GetType().Name;
            _logger.LogDebug(
                $"The message handler \"{messageHandlerName}\" started message handling at {startedAt:yyyy-MM-dd HH:mm:ss}.");

            var handleMessageTask = (Task)messageHandlerInvoker.DynamicInvoke(messageHandler, message, sender);

            return handleMessageTask.ContinueWith(t =>
            {
                if (!t.IsCompletedSuccessfully) return;

                var completedAt = DateTime.Now;
                var handledWithin = (completedAt - startedAt).TotalMilliseconds;

                _logger.LogDebug(
                    $"The message handler \"{messageHandlerName}\" completed message handling at {completedAt:yyyy-MM-dd HH:mm:ss} ({handledWithin} ms).");
            });
        }
    }
}