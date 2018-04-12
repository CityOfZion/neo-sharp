using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Test.Network
{
    [TestClass]
    public class UtNetworkManager
    {
        NetworkManager _uut;
        Mock<ILogger<NetworkManager>> _mockLogger;
        Mock<IServer> _mockServer;

        [TestInitialize]
        public void TestSetup()
        {
            _mockLogger = new Mock<ILogger<NetworkManager>>();
            _mockServer = new Mock<IServer>();

            _uut = new NetworkManager(_mockLogger.Object, _mockServer.Object);
        }

        [TestMethod]
        public void StartNetwork_Starts_Server()
        {
            _uut.StartNetwork();
            _mockServer.Verify(m => m.StartServer(), Times.Once);
        }

        [TestMethod]
        public void StopNetwork_Stops_Server()
        {
            _uut.StopNetwork();
            _mockServer.Verify(m => m.StopServer(), Times.Once);
        }

    }
}
