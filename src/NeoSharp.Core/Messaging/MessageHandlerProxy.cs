using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using NeoSharp.Core.DI;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging
{
    public class MessageHandlerProxy : IMessageHandler<Message>
    {
        private readonly IContainer _container;
        private readonly IDictionary<Type, Delegate> _messageHandlerInvokers;
        private static readonly Type _messageHandlerInterfaceType = typeof(IMessageHandler<>);

        public MessageHandlerProxy(IContainer container, IReadOnlyCollection<Type> messageHandlerTypes)
        {
            _container = container;

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
            var messageType = message.GetType();

            if (_messageHandlerInvokers.TryGetValue(messageType, out var messageHandlerInvoker) == false)
            {
                throw new InvalidOperationException($"The message of \"{messageType}\" type has no registered handlers.");
            }

            var messageHandler = _container.Resolve(_messageHandlerInterfaceType.MakeGenericType(messageType));

            return (Task)messageHandlerInvoker.DynamicInvoke(messageHandler, message, sender);
        }
    }
}