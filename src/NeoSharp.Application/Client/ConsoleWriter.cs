using System;

namespace NeoSharp.Application.Client
{
    public class ConsoleWriter : IConsoleWriter
    {
        /// <summary>
        /// Write output into console
        /// </summary>
        /// <param name="output">Output</param>
        public void Write(string output)
        {
            Console.Write(output);
        }
        /// <summary>
        /// Write line into console
        /// </summary>
        /// <param name="line">Line</param>
        /// <param name="style">Style</param>
        public void WriteLine(string line, ConsoleWriteStyle style = ConsoleWriteStyle.Output)
        {
            switch (style)
            {
                case ConsoleWriteStyle.Output:
                    {
                        if (Console.ForegroundColor != ConsoleColor.White)
                            Console.ForegroundColor = ConsoleColor.White;

                        break;
                    }
                case ConsoleWriteStyle.Input:
                    {
                        if (Console.ForegroundColor != ConsoleColor.Green)
                            Console.ForegroundColor = ConsoleColor.Green;

                        break;
                    }
                case ConsoleWriteStyle.Error:
                    {
                        if (Console.ForegroundColor != ConsoleColor.Red)
                            Console.ForegroundColor = ConsoleColor.Red;

                        break;
                    }
                case ConsoleWriteStyle.Information:
                    {
                        if (Console.ForegroundColor != ConsoleColor.Yellow)
                            Console.ForegroundColor = ConsoleColor.Yellow;

                        break;
                    }
            }

            Console.WriteLine(line);
        }
    }
}