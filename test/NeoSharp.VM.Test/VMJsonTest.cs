using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NeoSharp.VM.Test
{
    [TestClass]
    public class VMJsonTest : VMJsonTestBase
    {
        private readonly IVMFactory _factory = new NeoVM.NeoVMFactory();

        [TestMethod]
        public void TestJson()
        {
            foreach (var file in Directory.GetFiles("./Tests/", "*.json"))
            {
                ExecuteTest(_factory, File.ReadAllText(file));
            }
        }
    }
}