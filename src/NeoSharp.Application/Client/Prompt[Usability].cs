using NeoSharp.Application.Attributes;
using NeoSharp.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NeoSharp.Application.Client
{
    public partial class Prompt : IPrompt
    {
        /// <summary>
        /// Print help from Multiple commands
        /// </summary>
        /// <param name="cmds">Commands</param>
        void PrintHelp(IEnumerable<PromptCommandAttribute> cmds)
        {
            // Print help

            PromptCommandAttribute cmd = cmds.FirstOrDefault();

            if (cmd != null && !string.IsNullOrEmpty(cmd.Help))
            {
                _consoleWriter.WriteLine(cmd.Help, ConsoleOutputStyle.Information);

                _consoleWriter.WriteLine("");
                _consoleWriter.WriteLine("Examples:", ConsoleOutputStyle.Information);

                // How to use?

                List<string> modes = new List<string>();
                foreach (var v in cmds)
                {
                    string args = "";

                    if (v.Parameters != null && v.Parameters.Length > 0)
                    {
                        foreach (var par in v.Parameters)
                        {
                            string allowed = "";

                            if (par.ParameterType.IsEnum)
                            {
                                foreach (object o in Enum.GetValues(par.ParameterType))
                                    allowed += (allowed != "" ? "," : "") + o.ToString();

                                allowed = $" {par.Name}={allowed}";

                                if (!modes.Contains(allowed)) modes.Add(allowed);
                            }

                            if (par.HasDefaultValue)
                                args += $" [{par.Name}={(par.DefaultValue == null ? "NULL" : par.DefaultValue.ToString())}]";
                            else
                                args += $" {par.Name}";
                        }
                    }

                    _consoleWriter.WriteLine("  " + v.Command + args, ConsoleOutputStyle.Information);
                }

                if (modes.Count > 0)
                {
                    // Options

                    _consoleWriter.WriteLine("Options:", ConsoleOutputStyle.Information);
                    foreach (var par in modes)
                        _consoleWriter.WriteLine("  " + par, ConsoleOutputStyle.Information);
                }
            }
        }

        StreamWriter _record;

        /// <summary>
        /// Start current recording
        /// </summary>
        /// <param name="outputFile">File</param>
        [PromptCommand("record start", Help = "Record all commands in a file", Category = "Usability")]
        private void RecordStartCommand(FileInfo outputFile)
        {
            if (_record != null) throw (new Exception("Stop record first"));
            if (outputFile.Exists) throw (new Exception("Output file already exists"));

            _record = new StreamWriter(outputFile.FullName, false, Encoding.UTF8);
            OnCommandRequested += Prompt_OnCommandRequested;
        }

        /// <summary>
        /// Stop current recording
        /// </summary>
        [PromptCommand("record stop", Help = "Stop current record", Category = "Usability")]
        private void RecordStopCommand()
        {
            if (_record == null) throw (new Exception("Empty record"));

            OnCommandRequested -= Prompt_OnCommandRequested;

            _record.Flush();
            _record.Close();
            _record.Dispose();
            _record = null;
        }

        private void Prompt_OnCommandRequested(IPrompt prompt, string command)
        {
            if (string.Equals(command, "record stop"))
                return;

            _record.WriteLine(command);
        }

        /// <summary>
        /// Clear
        /// </summary>
        [PromptCommand("clear", Help = "clear output", Category = "Usability")]
        private void ClearCommand()
        {
            _consoleWriter.Clear();
        }

        /// <summary>
        /// Load commands from file
        /// </summary>
        /// <param name="file">File</param>
        [PromptCommand("load", Help = "Play stored commands", Category = "Usability")]
        private void LoadCommand(FileInfo file)
        {
            if (!file.Exists)
            {
                _consoleWriter.WriteLine("File not found", ConsoleOutputStyle.Error);
                return;
            }

            if (file.Length > 1024 * 1024)
            {
                _consoleWriter.WriteLine("The specified file is too large", ConsoleOutputStyle.Error);
                return;
            }

            var lines = File.ReadAllLines(file.FullName, Encoding.UTF8);
            _consoleReader.AppendInputs(lines);

            // Print result

            _consoleWriter.WriteLine($"Loaded inputs: {lines.Length}");
        }

        /// <summary>
        /// Exit prompt
        /// </summary>
        [PromptCommand("quit", Category = "Usability")]
        private void QuitCommand()
        {
            NetworkStopCommand();
            _exit = true;
        }

        /// <summary>
        /// Exit prompt
        /// </summary>
        [PromptCommand("exit", Category = "Usability")]
        private void ExitCommand()
        {
            NetworkStopCommand();
            _exit = true;
        }

        /// <summary>
        /// Show help
        /// </summary>
        [PromptCommand("help", Category = "Usability", Help = "Show help for commands")]
        private void HelpCommand([PromptCommandParameterBody]string command)
        {
            if (!string.IsNullOrWhiteSpace(command))
            {
                var cmdArgs = new List<CommandToken>();
                var cmds = SearchCommands(command, cmdArgs).ToArray();

                if (cmds.Length == 0)
                {
                    _consoleWriter.WriteLine("Command not found <" + command + ">", ConsoleOutputStyle.Error);
                }
                else PrintHelp(cmds);

                return;
            }

            HelpCommand();
        }

        /// <summary>
        /// Show help
        /// </summary>
        [PromptCommand("help", Category = "Usability", Help = "Show help for commands")]
        private void HelpCommand()
        {
            string lastCat = null, lastCom = null;
            foreach (string[] key in _commandCache.Keys.OrderBy(u => _commandCache[u].Category + "\n" + u))
            {
                var c = _commandCache[key];

                if (lastCat != c.Category)
                {
                    // Print category

                    lastCat = c.Category;
                    _consoleWriter.WriteLine(lastCat, ConsoleOutputStyle.Information);
                }

                string command = string.Join(" ", key);
                if (lastCom == command) continue;

                lastCom = command;
                _consoleWriter.WriteLine("  " + command);
            }
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