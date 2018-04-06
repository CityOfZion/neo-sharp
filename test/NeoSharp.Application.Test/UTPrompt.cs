using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Application.Client;
using NeoSharp.Core.Network;

namespace NeoSharp.Application.Test
{
    [TestClass]
    public class UTPrompt
    {
        Prompt uut;
        Mock<IConsoleReader> mockConsoleReader;
        Mock<IConsoleWriter> mockConsoleWriter;
        Mock<INetworkManager> mockNetworkManager;
        Mock<ILogger<Prompt>> mockLogger;

        [TestInitialize]
        public void TestSetup()
        {
            mockConsoleReader = new Mock<IConsoleReader>();
            mockConsoleWriter = new Mock<IConsoleWriter>();
            mockNetworkManager = new Mock<INetworkManager>();
            mockLogger = new Mock<ILogger<Prompt>>();
            uut = new Prompt(mockConsoleReader.Object, mockConsoleWriter.Object, mockLogger.Object, mockNetworkManager.Object);
        }

        private void startPromptCmdAndExit(string cmd)
        {
            mockConsoleReader.SetupSequence(m => m.ReadFromConsole()).Returns(cmd).Returns("exit");
            uut.StartPrompt(null);
            mockConsoleReader.Verify(m => m.ReadFromConsole(), Times.Exactly(2));
        }

        private void verifyNetworkManagerCalls(int startNetwork, int stopNetwork)
        {
            mockNetworkManager.Verify(m => m.StartNetwork(), Times.Exactly(startNetwork));
            mockNetworkManager.Verify(m => m.StopNetwork(), Times.Exactly(stopNetwork));      
        }

        [TestMethod]
        public void BlankCarriageReturn()
        {
            startPromptCmdAndExit("\r\n");           
            verifyNetworkManagerCalls(0, 1);
        }

        [TestMethod]
        public void StartCommand()
        {
            startPromptCmdAndExit("start");
            verifyNetworkManagerCalls(1, 1);
        }

        [TestMethod]
        public void StopCommand()
        {
            startPromptCmdAndExit("stop");
            verifyNetworkManagerCalls(0, 2);
        }

        [TestMethod]
        public void Exit()
        {
            mockConsoleReader.Setup(m => m.ReadFromConsole()).Returns("exit");
            uut.StartPrompt(null);
            mockConsoleReader.Verify(m => m.ReadFromConsole(), Times.Exactly(1));            
        }
    }
}
