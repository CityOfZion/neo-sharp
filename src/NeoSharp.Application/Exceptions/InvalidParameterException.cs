using System;
namespace NeoSharp.Application.Exceptions
{
    public class InvalidParameterException : Exception
    {
        public InvalidParameterException(string message) : base(message) 
        {
        }

        public InvalidParameterException()
        {
        }
    }
}
