using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var data = RandomByteArray(250);
            var msg = new MessageContainer();

            Assert.IsTrue(msg.GetMessage(0).SequenceEqual(new byte[] { }));

            msg.RegisterMessage(data);

            Assert.IsTrue(msg.GetMessage(0).SequenceEqual(data));
        }
    }
}