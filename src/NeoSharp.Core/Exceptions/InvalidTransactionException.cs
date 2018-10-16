using System;
namespace NeoSharp.Core.Exceptions
{
    public class InvalidTransactionException : Exception
    {
        public InvalidTransactionException(string message) : base(message)
        {
        }

        public InvalidTransactionException()
        {
        }
    }
}
