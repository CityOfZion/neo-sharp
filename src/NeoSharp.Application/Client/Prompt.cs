using Microsoft.Extensions.Logging;
using NeoSharp.Core.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NeoSharp.Core.Extensions;
using NeoSharp.Application.Attributes;
using System.Reflection;

namespace NeoSharp.Application.Client
{
    public class Prompt : IPrompt
    {
        #region Variables

        /// <summary>
        /// Exit flag
        /// </summary>
        private bool Exit = false;
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
        /// <summary>
        /// Command caché
        /// </summary>
        private static readonly Dictionary<string, PromptCommandAttribute> _CommandCache;

        #endregion

        #region Cache

        /// <summary>
        /// Static constructor
        /// </summary>
        static Prompt()
        {
            _CommandCache = new Dictionary<string, PromptCommandAttribute>();

            foreach (MethodInfo mi in typeof(Prompt).GetMethods
                (
                BindingFlags.NonPublic | BindingFlags.Public |
                BindingFlags.Instance | BindingFlags.Static
                ))
            {
                PromptCommandAttribute atr = mi.GetCustomAttribute<PromptCommandAttribute>();
                if (atr == null) continue;

                atr.Method = mi;

                foreach (string command in atr.Commands)
                    _CommandCache.Add(command.ToLowerInvariant(), atr);
            }
        }

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
            this._consoleWriter.WriteLine("Neo-Sharp", ConsoleOutputStyle.Prompt);

            while (!Exit)
            {
                var fullCmd = this._consoleReader.ReadFromConsole();
                if (string.IsNullOrWhiteSpace(fullCmd)) continue;

                PromptCommandAttribute cmd = null;

                try
                {
                    // Parse arguments

                    List<string> cmdArgs = new List<string>(SplitCommandLine(fullCmd));
                    if (cmdArgs.Count <= 0) continue;

                    // Process command

                    if (!_CommandCache.TryGetValue(cmdArgs.First().ToLowerInvariant(), out cmd))
                    {
                        throw (new Exception("Command not found"));
                    }

                    // Get command

                    cmd.Method.Invoke(this, cmd.ConvertToArguments(cmdArgs.Skip(1).ToArray()));
                }
                catch (Exception e)
                {
                    _consoleWriter.WriteLine(e.Message, ConsoleOutputStyle.Error);

                    // Print help

                    if (cmd != null && !string.IsNullOrEmpty(cmd.Help))
                        _consoleWriter.WriteLine(cmd.Help, ConsoleOutputStyle.Information);
                }
            }

            _consoleWriter.WriteLine("Exiting", ConsoleOutputStyle.Information);
        }

        #region Commands

        /// <summary>
        /// Load commands from file
        /// </summary>
        /// <param name="file">File</param>
        [PromptCommand("load", Help = "load <filename>\nPlay stored commands")]
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

            string[] lines = File.ReadAllLines(file.FullName, Encoding.UTF8);
            _consoleReader.AppendInputs(lines);

            // Print result

            _consoleWriter.WriteLine($"Loaded inputs: {lines.Length.ToString()}");
        }

        [PromptCommand("start")]
        private void StartCommand()
        {
            this._networkManager.StartNetwork();
        }

        [PromptCommand(new string[] { "exit", "quit" })]
        private void ExitCommand()
        {
            StopCommand();
            this.Exit = true;
        }

        [PromptCommand("stop")]
        private void StopCommand()
        {
            this._networkManager.StopNetwork();
        }

        [PromptCommand("help")]
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
