using System;
namespace NeoSharp.Core.Exceptions
{
    public class UnknownPersistentProvider : Exception
    {
        public UnknownPersistentProvider(string message) : base(message)
        {
        }
    }
}
