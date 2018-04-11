using System;

namespace NeoSharp.Application.Client
{
    public class ConsoleWriter : IConsoleWriter
    {
        /// <summary>
        /// Get current cursor positon
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public void GetCursorPosition(out int x, out int y)
        {
            x = Console.CursorLeft;
            y = Console.CursorTop;
        }
        /// <summary>
        /// Set cursor position
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public void SetCursorPosition(int x, int y)
        {
            Console.CursorLeft = x;
            Console.CursorTop = y;
        }
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
                case ConsoleOutputStyle.Prompt:
                    {
                        if (Console.ForegroundColor != ConsoleColor.DarkGreen)
                            Console.ForegroundColor = ConsoleColor.DarkGreen;

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
        /// <summary>
        /// Create percent writer
        /// </summary>
        /// <param name="maxValue">Maximum value</param>
        /// <returns>Return Console percent writer</returns>
        public ConsolePercentWriter CreatePercent(int maxValue = 100)
        {
            return new ConsolePercentWriter(this, 0, maxValue);
        }
    }
}