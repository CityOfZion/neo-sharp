using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NeoSharp.VM.Test
{
    [TestClass]
    public class VMJsonTest : VMJsonTestBase
    {
        [TestMethod]
        public void TestJson()
        {
            foreach(var file in new string[]
            {
                "./Tests/sample1.json",
                "./Tests/sample2.json",
            })

            ExecuteTest(File.ReadAllText(file));
        }
    }
}