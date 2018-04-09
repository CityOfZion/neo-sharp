using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace NeoSharp.Application.Client
{
    public class ConsoleReader : IConsoleReader
    {
        #region Constants

        /// <summary>
        /// Prompt
        /// </summary>
        private const string _readPrompt = "neo#> ";

        #endregion

        #region Variables

        private readonly IConsoleWriter _consoleWriter;
        private readonly List<string> _manualInputs;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="consoleWriterInit">Console writer</param>
        public ConsoleReader(IConsoleWriter consoleWriterInit)
        {
            _consoleWriter = consoleWriterInit;
            _manualInputs = new List<string>();
        }

        /// <summary>
        /// Append inputs
        /// </summary>
        /// <param name="inputs">Inputs</param>
        public void AppendInputs(params string[] inputs)
        {
            if (inputs == null) return;

            // Add non-empty entries to manual inputs

            _manualInputs.AddRange(inputs.Where(u => !string.IsNullOrEmpty(u)));
        }
        /// <summary>
        /// Read password
        /// </summary>
        /// <returns>Reteurn Secure string password</returns>
        public SecureString ReadPassword()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            SecureString pwd = new SecureString();

            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.RemoveAt(pwd.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    pwd.AppendChar(i.KeyChar);
                    Console.Write("*");
                }
            }

            return pwd;
        }
        /// <summary>
        /// Read string from console
        /// </summary>
        /// <returns>Returns the readed string</returns>
        public string ReadFromConsole()
        {
            // Write prompt

            if (Console.ForegroundColor != ConsoleColor.DarkGreen)
                Console.ForegroundColor = ConsoleColor.DarkGreen;

            _consoleWriter.Write(_readPrompt);

            // If have something loaded

            Console.ForegroundColor = ConsoleColor.Green;

            if (_manualInputs.Count > 0)
            {
                // Get first loaded command

                string input = _manualInputs[0];
                _manualInputs.RemoveAt(0);

                if (!string.IsNullOrEmpty(input))
                {
                    // Print it

                    _consoleWriter.WriteLine(input, ConsoleOutputStyle.Input);

                    // Use it

                    return input;
                }
            }

            // Read from console

            return Console.ReadLine();
        }
    }
}