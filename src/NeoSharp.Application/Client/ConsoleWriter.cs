using System;

namespace NeoSharp.Application.Client
{
    public class ConsoleWriter : IConsoleWriter
    {     
        public void Write(string output)
        {
            Console.Write(output);
        }

        public void WriteLine(string output)
        {
            Console.WriteLine(output);
        }
    }
}
