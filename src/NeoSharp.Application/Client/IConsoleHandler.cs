using System.Security;

namespace NeoSharp.Application.Client
{
    public interface IConsoleHandler 
    {
        /// <summary>
        /// State
        /// </summary>
        ConsoleReaderState State { get; }

        /// <summary>
        /// Key available
        /// </summary>
        bool KeyAvailable { get; }

        /// <summary>
        /// Read password
        /// </summary>
        /// <param name="promptLabel">Prompt label</param>
        /// <returns>Reteurn Secure string password</returns>
        SecureString ReadPassword(string promptLabel = "Password: ");

        /// <summary>
        /// Read string from console
        /// </summary>
        /// <param name="autocomplete">Autocomplete</param>
        /// <returns>Returns the readed string</returns>
        string ReadFromConsole(IAutoCompleteHandler autocomplete = null);

        /// <summary>
        /// Append inputs
        /// </summary>
        /// <param name="inputs">Inputs</param>
        void AppendInputs(params string[] inputs);

        /// <summary>
        /// Beep
        /// </summary>
        void Beep();

        /// <summary>
        /// Clear
        /// </summary>
        void Clear();

        /// <summary>
        /// Apply style
        /// </summary>
        /// <param name="style">Style</param>
        void ApplyStyle(ConsoleOutputStyle style);

        /// <summary>
        /// Get current cursor positon
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        void GetCursorPosition(out int x, out int y);

        /// <summary>
        /// Set cursor position
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        void SetCursorPosition(int x, int y);

        /// <summary>
        /// Create percent writer
        /// </summary>
        /// <param name="maxValue">Maximum value</param>
        /// <returns>Return Console percent writer</returns>
        ConsolePercentWriter CreatePercent(long maxValue = 100);

        /// <summary>
        /// Write prompt
        /// </summary>
        void WritePrompt();

        /// <summary>
        /// Write output into console
        /// </summary>
        /// <param name="output">Output</param>
        /// <param name="style">Style</param>
        void Write(string output, ConsoleOutputStyle style = ConsoleOutputStyle.Output);

        /// <summary>
        /// Write line into console
        /// </summary>
        /// <param name="line">Line</param>
        /// <param name="style">Style</param>
        void WriteLine(string line, ConsoleOutputStyle style = ConsoleOutputStyle.Output);

        /// <summary>
        /// Write object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="output">Output</param>
        /// <param name="style">Style</param>
        void WriteObject(object obj, PromptOutputStyle output, ConsoleOutputStyle style = ConsoleOutputStyle.Output);
    }
}