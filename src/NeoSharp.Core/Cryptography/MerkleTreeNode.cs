using NeoSharp.Core.Types;

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
        public MerkleTreeNode()
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hash">Hash</param>
        public MerkleTreeNode(UInt256 hash)
        {
            Hash = hash;
        }
    }
}