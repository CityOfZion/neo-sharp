using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.DI;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;
using NeoSharp.Core.Test.ExtensionMethods;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Messaging
{
    [TestClass]
    public class UtMessageHandlerProxy : TestBase
    {
        //private class NullVersionMessageHandler : IMessageHandler<VersionMessage>
        //{
        //    public Task Handle(VersionMessage message, IPeer sender)
        //    {
        //        return Task.CompletedTask;
        //    }
        //}

        //[TestMethod]
        //public void Can_delegate_message_handling()
        //{
        //    // Arrange
        //    var serverContextMock = AutoMockContainer
        //        .GetMock<IServerContext>()
        //        .SetupDefaultServerContext();

        //    var containerMock = AutoMockContainer.GetMock<IContainer>();

        //    containerMock
        //        .Setup(c => c.Resolve(It.IsAny<Type>()))
        //        .Returns(() => new NullVersionMessageHandler());

        //    var loggerMock = AutoMockContainer.GetMock<ILogger<MessageHandlerProxy>>();

        //    var messageHandlerProxy = new MessageHandlerProxy(containerMock.Object, new[] { typeof(NullVersionMessageHandler) }, loggerMock.Object);

        //    // Act
        //    var task = messageHandlerProxy.Handle(new VersionMessage(serverContextMock.Object.Version), null);
        //    task.Wait();

        //    // Assert
        //    task.IsCompletedSuccessfully.Should().Be(true);
        //}

        //[TestMethod]
        //public void Throw_on_receiving_of_unknown_message()
        //{
        //    // Arrange
        //    var serverContextMock = AutoMockContainer
        //        .GetMock<IServerContext>()
        //        .SetupDefaultServerContext();

        //    var containerMock = AutoMockContainer.GetMock<IContainer>();
        //    var loggerMock = AutoMockContainer.GetMock<ILogger<MessageHandlerProxy>>();
        //    var messageHandlerProxy = new MessageHandlerProxy(containerMock.Object, new Type[0], loggerMock.Object);

        //    // Act
        //    Action a = () => messageHandlerProxy.Handle(new VersionMessage(serverContextMock.Object.Version), null);

        //    // Assert
        //    a.Should().Throw<Exception>();
        //}
    }
}