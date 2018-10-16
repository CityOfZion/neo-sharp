using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NeoSharp.Application.Attributes;
using NeoSharp.Application.Exceptions;
using NeoSharp.Application.Extensions;
using NeoSharp.Application.Providers;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Network;
using NeoSharp.Core.Types;

namespace NeoSharp.Application.Client
{
    public partial class Prompt : IPrompt
    {
        #region Variables

        /// <summary>
        /// Exit flag
        /// </summary>
        private bool _exit;
        /// <summary>
        /// Network manager
        /// </summary>
        private readonly INetworkManager _networkManager;
        /// <summary>
        /// Console Handler
        /// </summary>
        private readonly IConsoleHandler _consoleHandler;
        /// <summary>
        /// Logger
        /// </summary>
        private readonly Core.Logging.ILogger<Prompt> _logger;
        /// <summary>
        /// Blockchain
        /// </summary>
        private readonly IBlockchain _blockchain;
        /// <summary>
        /// Command cache
        /// </summary>
        private readonly IDictionary<string[], PromptCommandAttribute> _commandCache;
        /// <summary>
        /// Autocomplete handler
        /// </summary>
        private readonly IAutoCompleteHandler _commandAutocompleteCache;
        /// <summary>
        /// Log for output
        /// </summary>
        private readonly ILogBag _logs;
        /// <summary>
        /// Prompt user variables
        /// </summary>
        private readonly IPromptUserVariables _variables;

        public delegate void delOnCommandRequested(IPrompt prompt, PromptCommandAttribute cmd, string commandLine);
        public event delOnCommandRequested OnCommandRequested;

        private static readonly Dictionary<LogLevel, ConsoleOutputStyle> _logStyle = new Dictionary<LogLevel, ConsoleOutputStyle>()
        {
            { LogLevel.Critical, ConsoleOutputStyle.Error },
            { LogLevel.Error, ConsoleOutputStyle.Error },
            { LogLevel.Information, ConsoleOutputStyle.Log },
            { LogLevel.None, ConsoleOutputStyle.Log },
            { LogLevel.Trace, ConsoleOutputStyle.Log },
            { LogLevel.Warning, ConsoleOutputStyle.Log },
            { LogLevel.Debug, ConsoleOutputStyle.Log }
        };

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="controllers">Controllers</param>
        /// <param name="variables">Variables</param>
        /// <param name="logs">Logs</param>
        /// <param name="networkManager">Network manger</param>
        /// <param name="consoleHandler">Console handler</param>
        /// <param name="logger">Logger</param>
        /// <param name="blockchain">Blockchain</param>
        /// <param name="vmFactory">VM Factory</param>
        public Prompt
            (
            IEnumerable<IPromptController> controllers,
            IPromptUserVariables variables,
            ILogBag logs,
            INetworkManager networkManager,
            IConsoleHandler consoleHandler,
            Core.Logging.ILogger<Prompt> logger,
            IBlockchain blockchain
            )
        {
            _networkManager = networkManager;
            _consoleHandler = consoleHandler;
            _logger = logger;
            _blockchain = blockchain;
            _logs = logs;
            _variables = variables;

            // Get controllers

            _commandAutocompleteCache = new AutoCommandComplete();
            _commandCache = new Dictionary<string[], PromptCommandAttribute>();
            _commandCache.Cache(this, _commandAutocompleteCache);

            foreach (var controller in controllers)
            {
                _commandCache.Cache(controller, _commandAutocompleteCache);
            }

            // Help autocomplete

            var ls = new List<string>();

            foreach (var keys in _commandCache.Keys) foreach (var key in keys)
                {
                    if (_commandCache.TryGetValue(keys, out var value) && !ls.Contains(value.Command))
                    {
                        ls.Add(value.Command);
                    }
                }

            CommandAutoCompleteAttribute.Availables = ls.OrderBy(u => u).ToArray();
        }

        /// <inheritdoc />
        public void StartPrompt(string[] args)
        {
            _logger.LogInformation("Starting Prompt");
            _consoleHandler.WriteLine("Neo-Sharp", ConsoleOutputStyle.Prompt);

            if (args != null)
            {
                // Append arguments as inputs

                _consoleHandler.AppendInputs(args.Where(u => !u.StartsWith("#")).ToArray());
            }

            _blockchain.InitializeBlockchain().Wait();

            while (!_exit)
            {
                // Read log buffer

                while (_logs.TryTake(out var log))
                {
                    _consoleHandler.WriteLine
                        (
                        "[" + log.Level + (string.IsNullOrEmpty(log.Category) ? "" : "-" + log.Category) + "] " +
                        log.MessageWithError, _logStyle[log.Level]
                        );
                }

                // Read input

                var fullCmd = _consoleHandler.ReadFromConsole(_commandAutocompleteCache);

                if (string.IsNullOrWhiteSpace(fullCmd))
                {
                    continue;
                }

                fullCmd = _variables.Replace(fullCmd);

                _logger.LogInformation("Execute: " + fullCmd);

                Execute(fullCmd);
            }

            _consoleHandler.WriteLine("Exiting", ConsoleOutputStyle.Information);
        }

        /// <inheritdoc />
        public bool Execute(string command)
        {
            command = command.Trim();
            PromptCommandAttribute[] cmds = null;

            try
            {
                // Parse arguments

                var cmdArgs = new List<CommandToken>(command.SplitCommandLine());
                cmds = _commandCache.SearchCommands(cmdArgs).ToArray();
                var cmd = cmds.SearchRightCommand(cmdArgs, null, out var args);

                if (cmd == null)
                {
                    if (cmds.Length > 0)
                    {
                        throw new InvalidParameterException($"Wrong parameters for <{cmds.FirstOrDefault().Command}>");
                    }

                    throw new InvalidPromptCommandException($"Command not found <{command}>");
                }

                // Get command

                lock (_consoleHandler)
                {
                    // Raise event

                    OnCommandRequested?.Invoke(this, cmd, command);

                    // Invoke

                    var ret = cmd.Method.Invoke(cmd.Instance, args);

                    if (ret is Task task)
                    {
                        task.Wait();
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                var msg = e.InnerException != null ? e.InnerException.Message : e.Message;
                _consoleHandler.WriteLine(msg, ConsoleOutputStyle.Error);
#if DEBUG
                _consoleHandler.WriteLine(e.ToString(), ConsoleOutputStyle.Error);
#endif

                PrintHelp(cmds);
                return false;
            }
        }
    }
}