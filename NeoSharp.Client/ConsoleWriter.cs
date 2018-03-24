using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Client
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
