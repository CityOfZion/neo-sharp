using System;

namespace NeoSharp.Core.Wallet.Exceptions
{
    public class WalletNotOpenException : Exception
    {
        public WalletNotOpenException() : base("Open the wallet before this operation")
        {
        }
    }
}