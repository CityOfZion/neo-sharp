using System.Security;

namespace NeoSharp.Application.Client
{
    public interface IConsoleReader
    {
        /// <summary>
        /// Read password
        /// </summary>
        /// <returns>Reteurn Secure string password</returns>
        SecureString ReadPassword();
        /// <summary>
        /// Read string from console
        /// </summary>
        /// <returns>Returns the readed string</returns>
        string ReadFromConsole();
        /// <summary>
        /// Append inputs
        /// </summary>
        /// <param name="inputs">Inputs</param>
        void AppendInputs(params string[] inputs);
    }
}