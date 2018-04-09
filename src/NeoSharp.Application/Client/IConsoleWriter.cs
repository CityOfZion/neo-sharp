namespace NeoSharp.Application.Client
{
    public interface IConsoleWriter
    {
        /// <summary>
        /// Write output into console
        /// </summary>
        /// <param name="output">Output</param>
        void Write(string output, ConsoleOutputStyle style = ConsoleOutputStyle.Output);
        /// <summary>
        /// Write line into console
        /// </summary>
        /// <param name="line">Line</param>
        /// <param name="style">Style</param>
        void WriteLine(string line, ConsoleOutputStyle style = ConsoleOutputStyle.Output);
    }
}