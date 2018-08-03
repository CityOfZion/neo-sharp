using System.IO;
using System.Linq;
using System.Text;
using NeoSharp.Application.Attributes;
using NeoSharp.Application.Client;

namespace NeoSharp.Application.Controllers
{
    public class PromptUsabilityController : IPromptController
    {
        #region Private fields

        private readonly IPromptUserVariables _variables;
        private readonly IConsoleWriter _consoleWriter;
        private readonly IConsoleReader _consoleReader;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="variables">Variables</param>
        /// <param name="consoleWriter">Console writter</param>
        /// <param name="consoleReader">Console reader</param>
        public PromptUsabilityController
            (
            IPromptUserVariables variables,
            IConsoleWriter consoleWriter, IConsoleReader consoleReader
            )
        {
            _variables = variables;
            _consoleReader = consoleReader;
            _consoleWriter = consoleWriter;
        }

        /// <summary>
        /// Clear
        /// </summary>
        [PromptCommand("clear", Help = "clear output", Category = "Usability")]
        public void ClearCommand()
        {
            _consoleWriter.Clear();
        }

        /// <summary>
        /// Set
        /// </summary>
        [PromptCommand("set", Help = "set a variable", Category = "Usability")]
        public void SetCommand(string varName, [PromptCommandParameterBody]string value)
        {
            _variables.Add(varName, value);
        }

        /// <summary>
        /// Unset
        /// </summary>
        [PromptCommand("unset", Help = "unset a variable", Category = "Usability")]
        public void UnsetCommand(string varName)
        {
            _variables.Remove(varName);
        }

        /// <summary>
        /// List variables
        /// </summary>
        [PromptCommand("variables", Help = "View register variables", Category = "Usability")]
        public void VariablesCommand()
        {
            foreach (var keyValue in _variables)
            {
                _consoleWriter.Write(keyValue.Key + "  ", ConsoleOutputStyle.Output);
                _consoleWriter.WriteLine(keyValue.Value, ConsoleOutputStyle.Information);
            }
        }

        /// <summary>
        /// Load commands from file
        /// </summary>
        /// <param name="commandsFile">File</param>
        [PromptCommand("load", Help = "Play stored commands", Category = "Usability")]
        public void LoadCommand(FileInfo commandsFile)
        {
            if (!commandsFile.Exists)
            {
                _consoleWriter.WriteLine("File not found", ConsoleOutputStyle.Error);
                return;
            }

            if (commandsFile.Length > 1024 * 1024)
            {
                _consoleWriter.WriteLine("The specified file is too large", ConsoleOutputStyle.Error);
                return;
            }

            var lines = File.ReadAllLines(commandsFile.FullName, Encoding.UTF8);
            _consoleReader.AppendInputs(lines.Where(u => !u.StartsWith("#")).ToArray());

            // Print result

            _consoleWriter.WriteLine($"Loaded inputs: {lines.Length}");
        }

        /*
        TODO
        notifications {block_number or address}
        mem
        config debug {on/off}
        config sc-events {on/off}
        config maxpeers {num_peers}
        debugstorage {on/off/reset} 
        */
    }
}