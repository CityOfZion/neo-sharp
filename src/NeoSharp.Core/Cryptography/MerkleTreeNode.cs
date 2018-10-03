using System.Collections.Generic;
using NeoSharp.Types;

namespace NeoSharp.Core.Cryptography
{
    public class MerkleTreeNode
    {
        /// <summary>
        /// Node hash
        /// </summary>
        public UInt256 Hash;
        /// <summary>
        /// Parent node
        /// </summary>
        public MerkleTreeNode Parent;
        /// <summary>
        /// Left child node
        /// </summary>
        public MerkleTreeNode LeftChild;
        /// <summary>
        /// Right child node
        /// </summary>
        public MerkleTreeNode RightChild;
        /// <summary>
        /// Is root
        /// </summary>
        public bool IsRoot => Parent == null;
        /// <summary>
        /// Is leaf
        /// </summary>
        public bool IsLeaf => LeftChild == null && RightChild == null;

        /// <summary>
        /// Constructor
        /// </summary>
        public MerkleTreeNode() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hash">Hash</param>
        public MerkleTreeNode(UInt256 hash)
        {
            Hash = hash;
        }

        /// <summary>
        /// Get leafs form node
        /// </summary>
        /// <param name="node">Node to start</param>
        /// <returns>Enumerate leafs</returns>
        private static IEnumerable<MerkleTreeNode> GetLeafs(MerkleTreeNode node)
        {
            if (node == null) yield break;
            if (node.IsLeaf) yield return node;

            if (node.LeftChild != null)
            {
                foreach (var a in GetLeafs(node.LeftChild))
                {
                    yield return a;
                }
            }
            if (node.RightChild != null)
            {
                foreach (var a in GetLeafs(node.RightChild))
                {
                    yield return a;
                }
            }
        }

        /// <summary>
        /// Get leafs from current node
        /// </summary>
        /// <returns>Enumerate leafs</returns>
        public IEnumerable<MerkleTreeNode> GetLeafs()
        {
            return GetLeafs(this);
        }
    }
}