using System;
namespace NeoSharp.Application.Exceptions
{
    public class InvalidPromptCommandException : Exception
    {
        public InvalidPromptCommandException(string message) : base(message)
        {
        }
    }
}
