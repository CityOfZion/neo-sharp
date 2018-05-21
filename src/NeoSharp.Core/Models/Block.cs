using System;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Block : BlockHeader
    {
        public Transaction[] Transactions;
    }
}