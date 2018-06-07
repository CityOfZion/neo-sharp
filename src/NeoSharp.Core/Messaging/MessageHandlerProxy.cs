using NeoSharp.Core.Caching;
using NeoSharp.Core.DI;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace NeoSharp.Core.Messaging
{
    public class MessageHandlerProxy : IMessageHandler<Message>
    {
        #region Variables

        private readonly IContainer _container;
        private readonly ILogger<MessageHandlerProxy> _logger;
        private readonly IReadOnlyDictionary<Type, Delegate> _messageHandlerInvokers;
        private object[] _reflectionCache;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container">Container</param>
        /// <param name="messageHandlerTypes">MessageHandler types</param>
        /// <param name="logger">Logger</param>
        public MessageHandlerProxy(IContainer container, IEnumerable<Type> messageHandlerTypes, ILogger<MessageHandlerProxy> logger)
        {
            _container = container;
            _logger = logger;
            _messageHandlerInvokers = messageHandlerTypes
                .Select(CreateMessageHandlerInvoker)
                .ToDictionary(x => x.MessageType, x => x.MessageHandlerInvoker);
        }

        /// <summary>
        /// Handle message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="sender">Sender</param>
        /// <returns>Task</returns>
        public Task Handle(Message message, IPeer sender)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            if (_reflectionCache == null)
            {
                // Create cache

                byte max = 0;
                var cache = ReflectionCache<MessageCommand>.CreateFromEnum<MessageCommand>();
                var r = new object[byte.MaxValue];

                foreach (MessageCommand v in Enum.GetValues(typeof(MessageCommand)))
                {
                    if (!cache.TryGetValue(v, out Type entry)) continue;

                    byte val = (byte)v;
                    r[val] = _container.Resolve(typeof(IMessageHandler<>).MakeGenericType(entry));
                    max = Math.Max(max, val);
                }

                Array.Resize(ref r, max + 1);
                _reflectionCache = r;
            }

            // Extract handler

            var messageType = message.GetType();
            var messageHandler = _reflectionCache[(byte)message.Command];

            if (messageHandler == null)
                throw new InvalidOperationException($"The message of \"{messageType}\" type has no registered handlers.");

            if (!_messageHandlerInvokers.TryGetValue(messageType, out var messageHandlerInvoker))
            {
                throw new InvalidOperationException(
                    $"The message of \"{messageType}\" type has no registered handlers.");
            }

            // Log start

            var startedAt = DateTime.Now;
            var messageHandlerName = messageHandler.GetType().Name;
            _logger.LogDebug($"The message handler \"{messageHandlerName}\" started message handling at {startedAt:yyyy-MM-dd HH:mm:ss}.");

            // Execute

            var handleMessageTask = (Task)messageHandlerInvoker.DynamicInvoke(messageHandler, message, sender);

            return handleMessageTask.ContinueWith(t =>
            {
                if (!t.IsCompletedSuccessfully) return;

                // Log end

                var completedAt = DateTime.Now;
                var handledWithin = (completedAt - startedAt).TotalMilliseconds;

                _logger.LogDebug(
                    $"The message handler \"{messageHandlerName}\" completed message handling at {completedAt:yyyy-MM-dd HH:mm:ss} ({handledWithin} ms).");
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
    }
}