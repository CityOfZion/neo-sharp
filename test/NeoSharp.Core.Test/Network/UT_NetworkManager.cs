using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Test.Network
{
    [TestClass]
    public class UT_NetworkManager
    {
        NetworkManager uut;
        Mock<ILogger<NetworkManager>> mockLogger;
        Mock<IServer> mockServer;

        [TestInitialize]
        public void TestSetup()
        {
            mockLogger = new Mock<ILogger<NetworkManager>>();
            mockServer = new Mock<IServer>();

            uut = new NetworkManager(mockLogger.Object, mockServer.Object);
        }

        [TestMethod]
        public void StartNetwork_Starts_Server()
        {
            uut.StartNetwork();
            mockServer.Verify(m => m.StartServer(), Times.Once);
        }

        [TestMethod]
        public void StopNetwork_Stops_Server()
        {
            uut.StopNetwork();
            mockServer.Verify(m => m.StopServer(), Times.Once);
        }

    }
}
