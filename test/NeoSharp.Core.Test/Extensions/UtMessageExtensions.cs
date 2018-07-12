using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;

namespace NeoSharp.Core.Test.Extensions
{
    [TestClass]
    public class UtMessageExtensions
    {
        [TestMethod]
        public void Message_is_Handshake()
        {
            var messageHandlerTypes = typeof(Message).Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableToGenericType(typeof(Message<>))
                 && (t == typeof(VersionMessage) || t == typeof(VerAckMessage)))
                .ToArray();

            foreach (var message in messageHandlerTypes)
            {
                var m = (Message)Activator.CreateInstance(message);
                Assert.IsTrue(m.IsHandshakeMessage());
            }
        }

        [TestMethod]
        public void Message_is_not_Handshake()
        {
            var messageHandlerTypes = typeof(Message).Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableToGenericType(typeof(Message<>))
                 && (t != typeof(VersionMessage) && t != typeof(VerAckMessage)))
                .ToArray();

            foreach (var message in messageHandlerTypes)
            {
                var m = (Message)Activator.CreateInstance(message);
                Assert.IsFalse(m.IsHandshakeMessage());
            }
        }
    }
}