using System;

namespace NeoSharp.Application.Client
{
    public class ConsoleWriter : IConsoleWriter
    {
        /// <summary>
        /// Apply style
        /// </summary>
        /// <param name="style">Style</param>
        void ApplyStyle(ConsoleOutputStyle style)
        {
            switch (style)
            {
                case ConsoleOutputStyle.Output:
                    {
                        if (Console.ForegroundColor != ConsoleColor.White)
                            Console.ForegroundColor = ConsoleColor.White;

                        break;
                    }
                case ConsoleOutputStyle.Input:
                    {
                        if (Console.ForegroundColor != ConsoleColor.Green)
                            Console.ForegroundColor = ConsoleColor.Green;

                        break;
                    }
                case ConsoleOutputStyle.Error:
                    {
                        if (Console.ForegroundColor != ConsoleColor.Red)
                            Console.ForegroundColor = ConsoleColor.Red;

                        break;
                    }
                case ConsoleOutputStyle.Information:
                    {
                        if (Console.ForegroundColor != ConsoleColor.Yellow)
                            Console.ForegroundColor = ConsoleColor.Yellow;

                        break;
                    }
            }
        }
        /// <summary>
        /// Write output into console
        /// </summary>
        /// <param name="output">Output</param>
        /// <param name="style">Style</param>
        public void Write(string output, ConsoleOutputStyle style = ConsoleOutputStyle.Output)
        {
            ApplyStyle(style);
            Console.Write(output);
        }
        /// <summary>
        /// Write line into console
        /// </summary>
        /// <param name="line">Line</param>
        /// <param name="style">Style</param>
        public void WriteLine(string line, ConsoleOutputStyle style = ConsoleOutputStyle.Output)
        {
            ApplyStyle(style);
            Console.WriteLine(line);
        }
    }
}