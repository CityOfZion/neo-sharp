using System;

namespace NeoSharp.Core.Wallet.Exceptions
{
    public class AccountsPasswordMismatchException : Exception
    {
        public AccountsPasswordMismatchException() : base("All accounts should have the same password")
        {
        }
    }
}