using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Application.Attributes;
using NeoSharp.Application.Extensions;
using NeoSharp.Application.Providers;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;

namespace NeoSharp.Application.Client
{
    public partial class Prompt : IPrompt
    {
        private StreamWriter _record;

        /// <summary>
        /// Print help from Multiple commands
        /// </summary>
        /// <param name="cmds">Commands</param>
        void PrintHelp(IEnumerable<PromptCommandAttribute> cmds)
        {
            // Print help

            var cmd = cmds.FirstOrDefault();

            if (cmd != null && !string.IsNullOrEmpty(cmd.Help))
            {
                _consoleHandler.WriteLine(cmd.Help, ConsoleOutputStyle.Information);

                _consoleHandler.WriteLine("");
                _consoleHandler.WriteLine("Examples:", ConsoleOutputStyle.Information);

                // How to use?

                var modes = new List<string>();
                foreach (var v in cmds)
                {
                    var args = "";

                    if (v.Parameters != null && v.Parameters.Length > 0)
                    {
                        foreach (var par in v.Parameters)
                        {
                            if (par.GetCustomAttribute<PromptHideHelpCommandAttribute>() != null) continue;

                            var allowed = "";

                            if (par.ParameterType.IsEnum)
                            {
                                foreach (var o in Enum.GetValues(par.ParameterType))
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

                    _consoleHandler.WriteLine("  " + v.Command + args, ConsoleOutputStyle.Information);
                }

                if (modes.Count > 0)
                {
                    // Options

                    _consoleHandler.WriteLine("Options:", ConsoleOutputStyle.Information);

                    foreach (var par in modes)
                    {
                        _consoleHandler.WriteLine("  " + par, ConsoleOutputStyle.Information);
                    }
                }
            }
        }

        /// <summary>
        /// Watch
        /// </summary>
        /// <param name="ms">Miliseconds</param>
        /// <param name="line">Line</param>
        [PromptCommand("watch", Help = "Watch command", Category = "Usability")]
        private void WatchCommand(uint ms, [PromptCommandParameterBody, CommandAutoComplete] string line)
        {
            if (line.Trim().ToLowerInvariant().StartsWith("watch"))
            {
                throw new InvalidOperationException();
            }

            using (var cancel = new CancellationTokenSource())
            {
                // Cancellation listener (wait any key)

                new Task(async () =>
                  {
                      while (!cancel.IsCancellationRequested)
                      {
                          if (_consoleHandler.KeyAvailable)
                          {
                              cancel.Cancel();
                              return;
                          }
                          await Task.Delay(10, cancel.Token);
                      }
                  }).Start();

                // watch logic

                while (!cancel.IsCancellationRequested)
                {
                    _consoleHandler.Clear();

                    if (!Execute(line))
                    {
                        break;
                    }

                    try
                    {
                        var ret = Task.Delay((int)ms, cancel.Token);
                        ret.Wait();
                    }
                    catch
                    {
                        break;
                    }
                }
            }
        }

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

        private void Prompt_OnCommandRequested(IPrompt prompt, PromptCommandAttribute cmd, string commandLine)
        {
            if (cmd.Command == "record stop")
                return;

            _record.WriteLine(commandLine);
        }

        /// <summary>
        /// Exit prompt
        /// </summary>
        [PromptCommand("quit", Category = "Usability")]
        [PromptCommand("exit", Category = "Usability")]
        public void ExitCommand()
        {
            _networkManager?.StopNetwork();
            _exit = true;
        }

        /// <summary>
        /// Show help
        /// </summary>
        [PromptCommand("help", Category = "Usability", Help = "Show help for commands")]
        public void HelpCommand([PromptCommandParameterBody, CommandAutoComplete]string command)
        {
            if (!string.IsNullOrWhiteSpace(command))
            {
                var cmdArgs = new List<CommandToken>(command.SplitCommandLine());
                var cmds = _commandCache.SearchCommands(cmdArgs).ToArray();

                if (cmds.Length == 0)
                {
                    _consoleHandler.WriteLine("Command not found <" + command + ">", ConsoleOutputStyle.Error);
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
        public void HelpCommand()
        {
            string lastCat = null, lastCom = null;
            foreach (var key in _commandCache.Keys.OrderBy(u => _commandCache[u].Category + "\n" + string.Join("", u)))
            {
                var c = _commandCache[key];

                if (lastCat != c.Category)
                {
                    // Print category

                    lastCat = c.Category;
                    _consoleHandler.WriteLine(lastCat, ConsoleOutputStyle.Information);
                }

                var command = string.Join(" ", key);
                if (lastCom == command) continue;

                lastCom = command;
                _consoleHandler.WriteLine("  " + command);
            }
        }
    }
}