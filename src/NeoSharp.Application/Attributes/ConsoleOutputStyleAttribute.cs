using System;

namespace NeoSharp.Application.Attributes
{
    internal class ConsoleOutputStyleAttribute : Attribute
    {
        /// <summary>
        /// Background color
        /// </summary>
        public ConsoleColor Background { get; set; } = ConsoleColor.Black;
        /// <summary>
        /// Foreground color
        /// </summary>
        public ConsoleColor Foreground { get; set; } = ConsoleColor.White;

        /// <summary>
        /// Apply style
        /// </summary>
        public void Apply()
        {
            if (Console.ForegroundColor != Foreground)
                Console.ForegroundColor = Foreground;

            if (Console.BackgroundColor != Background)
                Console.BackgroundColor = Background;
        }
    }
}