using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Application.Client;

namespace NeoSharp.Application.Test
{
    [TestClass]
    public class UT_ClientManager
    {
        ClientManager uut;
        Mock<IPrompt> mockPrompt;

        [TestInitialize]
        public void TestSetup()
        {
            mockPrompt = new Mock<IPrompt>();
            uut = new ClientManager(mockPrompt.Object);
        }

        [TestMethod]
        public void RunClient_NullArgs()
        {
            uut.RunClient(null);
            mockPrompt.Verify(m => m.StartPrompt(null), Times.Once);
        }

        [TestMethod]
        public void RunClient_EmptyArgs()
        {
            string[] strArgs = new string[0];
            uut.RunClient(strArgs);
            mockPrompt.Verify(m => m.StartPrompt(strArgs), Times.Once);
        }

        [TestMethod]
        public void RunClient_OneArg()
        {
            string[] strArgs = new string[1] { "CoZ" };            
            uut.RunClient(strArgs);
            mockPrompt.Verify(m => m.StartPrompt(strArgs), Times.Once);
        }

        [TestMethod]
        public void RunClient_MultiArgs()
        {
            string[] strArgs = new string[3] { "City", "of", "Zion" };            
            uut.RunClient(strArgs);
            mockPrompt.Verify(m => m.StartPrompt(strArgs), Times.Once);
        }
    }
}
