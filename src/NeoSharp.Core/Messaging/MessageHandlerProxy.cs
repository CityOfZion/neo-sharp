using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using NeoSharp.Core.Caching;
using NeoSharp.Core.DI;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging
{
    public class MessageHandlerProxy : IMessageHandler<Message>
    {
        #region Internal cache

        class Cache
        {
            #region Variables

            public readonly MessageCommand Command;
            public readonly object MessageHandler;
            public readonly string MessageHandlerName;
            public readonly Delegate HandlerInvoker;

            #endregion

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="command">Command</param>
            /// <param name="messageHandler">Message handler</param>
            /// <param name="messageHandlerInvoker">Message handler</param>
            public Cache(MessageCommand command, object messageHandler, Delegate messageHandlerInvoker)
            {
                Command = command;
                MessageHandler = messageHandler;
                MessageHandlerName = messageHandler == null ? "" : messageHandler.GetType().Name;
                HandlerInvoker = messageHandlerInvoker;
            }
        }

        #endregion

        #region Variables

        private readonly ILogger<MessageHandlerProxy> _logger;
        private readonly Cache[] _reflectionCache;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container">Container</param>
        /// <param name="messageHandlerTypes">MessageHandler types</param>
        /// <param name="logger">Logger</param>
        public MessageHandlerProxy(IContainer container, IEnumerable<Type> messageHandlerTypes, ILogger<MessageHandlerProxy> logger)
        {
            _logger = logger;
            _reflectionCache = GenerateCache(container, messageHandlerTypes.ToArray());
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

            // Extract handler

            var entry = _reflectionCache[(byte)message.Command];

            if (entry == null)
                throw new InvalidOperationException($"The message of \"{ message.GetType()}\" type has no registered handlers.");

            var startedAt = LogMessageHandlingStart(entry.MessageHandlerName);

            // Execute

            var handleMessageTask = (Task)entry.HandlerInvoker.DynamicInvoke(entry.MessageHandler, message, sender);

            return handleMessageTask.ContinueWith(t =>
            {
                if (!t.IsCompletedSuccessfully) return;

                LogMessageHandlingEnd(startedAt, entry.MessageHandlerName);
            });
        }

        /// <summary>
        /// Log start
        /// </summary>
        /// <param name="messageHandlerName">Message handler name</param>
        /// <returns>Return start date</returns>
        private DateTime LogMessageHandlingStart(string messageHandlerName)
        {
            var startedAt = DateTime.Now;

            _logger.LogDebug($"The message handler \"{messageHandlerName}\" started message handling at {startedAt:yyyy-MM-dd HH:mm:ss}.");

            return startedAt;
        }

        /// <summary>
        /// Log end
        /// </summary>
        /// <param name="startedAt">Start date</param>
        /// <param name="messageHandlerName">Message handler name</param>
        private void LogMessageHandlingEnd(DateTime startedAt, string messageHandlerName)
        {
            // Log end

            var completedAt = DateTime.Now;
            var handledWithin = (completedAt - startedAt).TotalMilliseconds;

            _logger.LogDebug(
                $"The message handler \"{messageHandlerName}\" completed message handling at {completedAt:yyyy-MM-dd HH:mm:ss} ({handledWithin} ms).");
        }

        /// <summary>
        /// Generate reflection cache
        /// </summary>
        /// <param name="container">Container</param>
        /// <param name="messageHandlerTypes">Message handler types</param>
        private Cache[] GenerateCache(IContainer container, Type[] messageHandlerTypes)
        {
            // Select Message Handler invokers

            var messageHandlerInvokers = messageHandlerTypes.Select(CreateMessageHandlerInvoker)
                .ToDictionary(x => x.MessageType, x => x.MessageHandlerInvoker);

            // Get Message types from the enum

            byte max = 0;
            var cache = ReflectionCache<MessageCommand>.CreateFromEnum<MessageCommand>();
            var r = new Cache[byte.MaxValue];

            // Assign the invokers and types to the specific command position in the cache

            foreach (MessageCommand v in Enum.GetValues(typeof(MessageCommand)))
            {
                if (!cache.TryGetValue(v, out Type centry) ||
                    !messageHandlerInvokers.TryGetValue(centry, out var messageHandlerInvoker))
                    continue;

                byte val = (byte)v;
                r[val] = new Cache(v, container.Resolve(typeof(IMessageHandler<>).MakeGenericType(centry)), messageHandlerInvoker);

                // Extract the max value of the command for trim the cache later

                max = Math.Max(max, val);
            }

            // Make the cache smaller (<255)

            Array.Resize(ref r, max + 1);
            return r;
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