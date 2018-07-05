using NeoSharp.Application.Attributes;
using NeoSharp.Core.Caching;
using System;

namespace NeoSharp.Application.Client
{
    public class ConsoleWriter : IConsoleWriter
    {
        #region Variables

        /// <summary>
        /// Prompt
        /// </summary>
        private const string ReadPrompt = "neo#> ";

        private object _lockObject = new object();

        static readonly ReflectionCache<ConsoleOutputStyle, ConsoleOutputStyleAttribute> _cache =
            ReflectionCache<ConsoleOutputStyle, ConsoleOutputStyleAttribute>.CreateFromEnum();

        #endregion

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
        public void ApplyStyle(ConsoleOutputStyle style)
        {
            _cache[style]?.Apply();
        }
        /// <summary>
        /// Beep
        /// </summary>
        public void Beep()
        {
            Console.Beep();
        }
        /// <summary>
        /// Clear
        /// </summary>
        public void Clear()
        {
            Console.Clear();
        }
        /// <summary>
        /// Write prompt
        /// </summary>
        public void WritePrompt()
        {
            Write(ReadPrompt, ConsoleOutputStyle.Prompt);
        }
        /// <summary>
        /// Write output into console
        /// </summary>
        /// <param name="output">Output</param>
        /// <param name="style">Style</param>
        public void Write(string output, ConsoleOutputStyle style = ConsoleOutputStyle.Output)
        {
            lock (_lockObject)
            {
                ApplyStyle(style);
                Console.Write(output);
            }
        }
        /// <summary>
        /// Write line into console
        /// </summary>
        /// <param name="line">Line</param>
        /// <param name="style">Style</param>
        public void WriteLine(string line, ConsoleOutputStyle style = ConsoleOutputStyle.Output)
        {
            lock (_lockObject)
            {
                ApplyStyle(style);
                Console.WriteLine(line);
            }
        }
        /// <summary>
        /// Create percent writer
        /// </summary>
        /// <param name="maxValue">Maximum value</param>
        /// <returns>Return Console percent writer</returns>
        public ConsolePercentWriter CreatePercent(long maxValue = 100)
        {
            return new ConsolePercentWriter(this, 0, maxValue);
        }
    }
}