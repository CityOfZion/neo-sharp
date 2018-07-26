using System.Security;

namespace NeoSharp.Application.Client
{
    public interface IConsoleReader
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
        SecureString ReadPassword(bool promptLabel = true);
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
    }
}