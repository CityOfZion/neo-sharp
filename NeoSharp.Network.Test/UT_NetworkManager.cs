using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NeoSharp.Network.Test
{
    [TestClass]
    public class UT_NetworkManager
    {
        NetworkManager uut;
        Mock<ILoggerFactory> mockLoggerFactory;
        Mock<IServer> mockServer;

        [TestInitialize]
        public void TestSetup()
        {
            mockLoggerFactory = new Mock<ILoggerFactory>();
            mockServer = new Mock<IServer>();

            uut = new NetworkManager(mockLoggerFactory.Object, mockServer.Object);
        }

        //[TestMethod]
        //public void StartNetwork_Starts_Server()
        //{
        //    uut.StartNetwork(); 
        //}

    }
}
