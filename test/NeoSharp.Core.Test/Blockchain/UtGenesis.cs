using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Blockchain;

namespace NeoSharp.Core.Test.Blockchain
{
    [TestClass]
    public class UtGenesis
    {
        [TestMethod]
        public void GenesisHashTest()
        {
            //On genesis it register BinarySerializer types
            Assert.AreEqual(Genesis.GenesisBlock.Hash.ToString(true), "0xd42561e3d30e15be6400b6df2f328e02d2bf6354c41dce433bc57687c82144bf");
            Assert.AreEqual(GenesisAssets.GoverningTokenRegisterTransaction.Hash.ToString(true), "0xc56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b");
            Assert.AreEqual(GenesisAssets.UtilityTokenRegisterTransaction.Hash.ToString(true), "0x602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7");
        }
    }
}