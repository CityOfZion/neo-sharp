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
        #region Variables

        private readonly IContainer _container;
        private readonly ILogger<MessageHandlerProxy> _logger;
        private readonly IReadOnlyDictionary<Type, Delegate> _messageHandlerInvokers;

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

            var messageType = message.GetType();
            var messageHandler = ResolveMessageHandler(messageType);
            var messageHandlerName = messageHandler.GetType().Name;
            var startedAt = LogMessageHandlingStart(messageHandlerName);
            var messageHandlerInvoker = GetMessageHandlerInvoker(messageType);
            var handleMessageTask = (Task)messageHandlerInvoker.DynamicInvoke(messageHandler, message, sender);

            return handleMessageTask.ContinueWith(t =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    LogMessageHandlingEnd(startedAt, messageHandlerName);
                    return;
                }

                if (t.IsFaulted)
                {
                    LogMessageHandlingEnd(startedAt, messageHandlerName, handleMessageTask.Exception);
                    return;
                }
            });
        }

        private static (Type MessageType, Delegate MessageHandlerInvoker) CreateMessageHandlerInvoker(Type messageHandlerType)
        {
            const string handleMethodName = nameof(IMessageHandler<Message>.Handle);
            const BindingFlags bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public;

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

        /// <summary>
        /// Log start
        /// </summary>
        /// <param name="messageHandlerName">Message handler name</param>
        /// <returns>Return start date</returns>
        private DateTime LogMessageHandlingStart(string messageHandlerName)
        {
            var startedAt = DateTime.Now;

            _logger.LogDebug(
                $"The message handler \"{messageHandlerName}\" started message handling at {startedAt:yyyy-MM-dd HH:mm:ss}.");

            return startedAt;
        }

        /// <summary>
        /// Log end with error
        /// </summary>
        /// <param name="startedAt">Start date</param>
        /// <param name="messageHandlerName">Message handler name</param>
        /// <param name="error">Error</param>
        private void LogMessageHandlingEnd(DateTime startedAt, string messageHandlerName, Exception error)
        {
            var completedAt = DateTime.Now;
            var handledWithin = (completedAt - startedAt).TotalMilliseconds;

            _logger.LogError(error,
                $"The message handler \"{messageHandlerName}\" faulted message handling at {completedAt:yyyy-MM-dd HH:mm:ss} ({handledWithin} ms).");
        }

        /// <summary>
        /// Log end
        /// </summary>
        /// <param name="startedAt">Start date</param>
        /// <param name="messageHandlerName">Message handler name</param>
        private void LogMessageHandlingEnd(DateTime startedAt, string messageHandlerName)
        {
            var completedAt = DateTime.Now;
            var handledWithin = (completedAt - startedAt).TotalMilliseconds;

            _logger.LogDebug(
                $"The message handler \"{messageHandlerName}\" completed message handling at {completedAt:yyyy-MM-dd HH:mm:ss} ({handledWithin} ms).");
        }
    }
}