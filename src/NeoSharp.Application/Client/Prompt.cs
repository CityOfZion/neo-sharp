using Microsoft.Extensions.Logging;
using NeoSharp.Core.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NeoSharp.Application.Client
{
    public class Prompt : IPrompt
    {
        #region Variables

        /// <summary>
        /// Console Reader
        /// </summary>
        private readonly IConsoleReader _consoleReader;
        /// <summary>
        /// Console Writer
        /// </summary>
        private readonly IConsoleWriter _consoleWriter;
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<Prompt> _logger;
        /// <summary>
        /// Network manager
        /// </summary>
        private readonly INetworkManager _networkManager;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="consoleReaderInit">Console reader init</param>
        /// <param name="consoleWriterInit">Console writer init</param>
        /// <param name="logger">Logger</param>
        /// <param name="networkManagerInit">Network manger init</param>
        public Prompt(IConsoleReader consoleReaderInit, IConsoleWriter consoleWriterInit, ILogger<Prompt> logger, INetworkManager networkManagerInit)
        {
            this._consoleReader = consoleReaderInit;
            this._consoleWriter = consoleWriterInit;
            this._logger = logger;
            this._networkManager = networkManagerInit;
        }

        /// <summary>
        /// Split a command line into enumerable
        ///     https://stackoverflow.com/a/24829691
        /// </summary>
        /// <param name="commandLine">Command line</param>
        /// <returns>Return the ienumerable result</returns>
        public static IEnumerable<string> SplitCommandLine(string commandLine)
        {
            bool inQuotes = false;
            bool isEscaping = false;

            return commandLine.Split(c =>
            {
                if (c == '\\' && !isEscaping) { isEscaping = true; return false; }

                if (c == '\"' && !isEscaping)
                    inQuotes = !inQuotes;

                isEscaping = false;

                return !inQuotes && char.IsWhiteSpace(c)/*c == ' '*/;
            })
                .Select(arg => arg.Trim().TrimMatchingQuotes('\"').Replace("\\\"", "\""))
                .Where(arg => !string.IsNullOrEmpty(arg));
        }

        public void StartPrompt(string[] args)
        {
            this._logger.LogInformation("Starting Prompt");
            this._consoleWriter.WriteLine("Neo-Sharp");

            var exit = false;
            while (!exit)
            {
                var cmd = this._consoleReader.ReadFromConsole();
                if (string.IsNullOrWhiteSpace(cmd)) continue;

                try
                {
                    // Parse arguments

                    List<string> cmdArgs = new List<string>(SplitCommandLine(cmd));
                    if (cmdArgs.Count <= 0) continue;

                    // Get command

                    cmd = cmdArgs.FirstOrDefault();
                    cmdArgs.RemoveAt(0);

                    // Process command

                    switch (cmd)
                    {
                        case "load":
                            {
                                LoadCommand(cmdArgs.ToArray());
                                break;
                            }
                        case "start":
                            {
                                StartCommand();
                                break;
                            }
                        case "stop":
                            {
                                StopCommand();
                                break;
                            }
                        case "help":
                            {
                                HelpCommand();
                                break;
                            }
                        case "exit":
                            {
                                StopCommand();
                                exit = true;
                                break;
                            }
                    }
                }
                catch (Exception e)
                {
                    _consoleWriter.WriteLine(e.Message, ConsoleOutputStyle.Error);
                }
            }

            _consoleWriter.WriteLine("Exiting", ConsoleOutputStyle.Information);
        }

        #region Commands

        /// <summary>
        /// Load commands from file
        /// </summary>
        /// <param name="args">Arguments</param>
        private void LoadCommand(string[] args)
        {
            if (args == null || args.Length <= 0)
            {
                _consoleWriter.WriteLine("File required", ConsoleOutputStyle.Error);
                return;
            }

            string file = args[0];
            if (!File.Exists(file))
            {
                _consoleWriter.WriteLine("File not found", ConsoleOutputStyle.Error);
                return;
            }

            if (new FileInfo(file).Length > 1024 * 1024)
            {
                _consoleWriter.WriteLine("The specified file is too large", ConsoleOutputStyle.Error);
                return;
            }

            string[] lines = File.ReadAllLines(file, Encoding.UTF8);
            _consoleReader.AppendInputs(lines);

            // Print result

            _consoleWriter.WriteLine($"Loaded inputs: {lines.Length.ToString()}");
        }

        private void StartCommand()
        {
            this._networkManager.StartNetwork();
        }

        private void StopCommand()
        {
            this._networkManager.StopNetwork();
        }

        private void HelpCommand()
        {
            _consoleWriter.WriteLine("load");
            _consoleWriter.WriteLine("start");
            _consoleWriter.WriteLine("stop");
            _consoleWriter.WriteLine("help");
            _consoleWriter.WriteLine("exit");
        }

        #endregion
    }
}
