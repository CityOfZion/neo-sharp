using System.Collections.Generic;
using System.Reflection;
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
        /// Read password
        /// </summary>
        /// <returns>Reteurn Secure string password</returns>
        SecureString ReadPassword();
        /// <summary>
        /// Read string from console
        /// </summary>
        /// <param name="autocomplete">Autocomplete</param>
        /// <returns>Returns the readed string</returns>
        string ReadFromConsole(IDictionary<string, List<ParameterInfo[]>> autocomplete = null);
        /// <summary>
        /// Append inputs
        /// </summary>
        /// <param name="inputs">Inputs</param>
        void AppendInputs(params string[] inputs);
    }
}