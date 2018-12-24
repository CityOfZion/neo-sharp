using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;
using NeoSharp.Core.SmartContract;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.SmartContracts
{
    [TestClass]
    public class UtMessageContainer : TestBase
    {
        [TestMethod]
        public void TestMessageContainer()
        {
            var tx = new MinerTransaction();
            var messageContainer = new MessageContainer(BinarySerializer.Default);

            Assert.IsTrue(messageContainer.GetMessageData(0).SequenceEqual(new byte[] { }));

            messageContainer.RegisterMessage(tx);

            Assert.IsTrue(messageContainer.GetMessageData(0).SequenceEqual(BinarySerializer.Default.Serialize(tx)));
        }
    }
}