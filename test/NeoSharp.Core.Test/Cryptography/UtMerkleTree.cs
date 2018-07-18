using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Cryptography
{
    [TestClass]
    public class UtMerkleTree : TestBase
    {
        private readonly UInt256[] _hashes = new UInt256[]
            {
                new UInt256("1a259dba256600620c6c91094f3a300b30f0cbaecee19c6114deffd3288957d7".HexToBytes()),
                new UInt256("0d40e64e3455677c1521870d6b732366e2434e18ffc4d2e10140a40131f96d4b".HexToBytes()),
                new UInt256("a3e5514bcfd79e7649e30fbf98826208f3da47833b4223b68cff2db23beb3801".HexToBytes()),
                new UInt256("e69601364bdac76eeebf277bea25b850aa4c7d9f7d28f670084f7ee801560ea5".HexToBytes())
            };

        private Crypto _crypto;

        [TestInitialize]
        public void Init()
        {
            _crypto = AutoMockContainer.Create<BouncyCastleCrypto>();
        }

        [TestMethod]
        public void Test_Tree()
        {
            // Act
            var root = MerkleTree.ComputeRoot(_hashes);
            var tree = MerkleTree.ComputeTree(_hashes);

            // Assert
            Assert.AreEqual(root.ToString(), "0x04948a9f4b7d5d1c7ed2cbd7f7034cfa095ce08d0b7a25959a083dc446aa8743");
            Assert.AreEqual(root, tree.Root.Hash);
            Assert.AreEqual(tree.Depth, 3);
            Assert.AreEqual(tree.Root.LeftChild.Hash.ToString(), "0xe2a1ce95dc4fbff3c66af5900ff2733d78c0eaab7c723c92f0a1e62f23f6049d");
            Assert.AreEqual(tree.Root.RightChild.Hash.ToString(), "0x6f143cb819dc8934ad357cb9b8185c084c7b82bbb173ef4d06b2acb09f70ad76");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Exception()
        {
            // Arrange
            var hashes = new UInt256[] { };

            // Act
            var tree = MerkleTree.ComputeTree(hashes);
        }

        [TestMethod]
        public void Search_Node()
        {
            // Act
            var tree = MerkleTree.ComputeTree(_hashes);
            var node = tree.Search(UInt256.Parse("d7578928d3ffde14619ce1ceaecbf0300b303a4f09916c0c62006625ba9d251a"));
            var nodeNone = tree.Search(UInt256.Parse("f6049de2a1ce95dc4fbff3c66af5900ff2733d78c0eaab7c723c92f0a1e62f23"));

            // Assert
            Assert.AreEqual(node.Hash.ToString(), "0xd7578928d3ffde14619ce1ceaecbf0300b303a4f09916c0c62006625ba9d251a");
            Assert.IsNull(nodeNone);
        }

        [TestMethod]
        public void Get_Leafs()
        {
            // Arrange
            var tree = MerkleTree.ComputeTree(_hashes);

            // Act
            var leafs = tree.Root.GetLeafs().Select(u => u.Hash).ToArray();

            // Assert
            Assert.IsTrue(_hashes.SequenceEqual(leafs));
            Assert.AreEqual(leafs.Length, (tree.Depth - 1) * 2);
        }
    }
}
