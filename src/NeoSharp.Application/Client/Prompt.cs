using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NeoSharp.Application.Attributes;
using NeoSharp.BinarySerialization;
using NeoSharp.BinarySerialization.DI;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.DI;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Network;
using NeoSharp.Core.Network.Rpc;
using NeoSharp.Core.Types;
using NeoSharp.Core.Wallet;

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
        /// Serializer
        /// </summary>
        private readonly IBinarySerializer _serializer;
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
        private readonly Core.Logging.ILogger<Prompt> _logger;
        /// <summary>
        /// Network manager
        /// </summary>
        private readonly INetworkManager _networkManager;
        /// <summary>
        /// Server
        /// </summary>
        private readonly IServer _server;
        /// <summary>
        /// Blockchain
        /// </summary>
        private readonly IBlockchain _blockchain;
        /// <summary>
        /// Rpc server
        /// </summary>
        private readonly IRpcServer _rpc;
        /// <summary>
        /// The wallet.
        /// </summary>
        private readonly IWalletManager _walletManager;
        /// <summary>
        /// Command cache
        /// </summary>
        private static readonly IDictionary<string[], PromptCommandAttribute> _commandCache;
        private static readonly IDictionary<string, List<ParameterInfo[]>> _commandAutocompleteCache;

        private readonly ILoggerFactoryExtended _loggerFactory;

        public delegate void delOnCommandRequested(IPrompt prompt, PromptCommandAttribute cmd, string commandLine);
        public event delOnCommandRequested OnCommandRequested;

        private readonly ConcurrentBag<LogEntry> _logs;

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

        #region Cache

        /// <summary>
        /// Static constructor
        /// </summary>
        static Prompt()
        {
            _commandCache = new Dictionary<string[], PromptCommandAttribute>();
            _commandAutocompleteCache = new Dictionary<string, List<ParameterInfo[]>>();

            foreach (var mi in typeof(Prompt).GetMethods
                (
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
                ))
            {
                var atr = mi.GetCustomAttribute<PromptCommandAttribute>();
                if (atr == null) continue;

                atr.SetMethod(mi);

                _commandCache.Add(atr.Commands, atr);

                if (_commandAutocompleteCache.ContainsKey(atr.Command))
                {
                    _commandAutocompleteCache[atr.Command].Add(mi.GetParameters());
                }
                else
                {
                    var ls = new List<ParameterInfo[]>
                    {
                        mi.GetParameters()
                    };
                    _commandAutocompleteCache.Add(atr.Command, ls);
                }
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="consoleReaderInit">Console reader init</param>
        /// <param name="consoleWriterInit">Console writer init</param>
        /// <param name="loggerFactory">Logget factory</param>
        /// <param name="logger">Logger</param>
        /// <param name="networkManagerInit">Network manger init</param>
        /// <param name="serverInit">Server</param>
        /// <param name="rpcInit">Rpc server</param>
        /// <param name="serializer">Binary serializer</param>
        /// <param name="blockchain">Blockchain</param>
        /// <param name="walletManager"></param>
        public Prompt(
            IConsoleReader consoleReaderInit,
            IConsoleWriter consoleWriterInit,
            ILoggerFactoryExtended loggerFactory,
            Core.Logging.ILogger<Prompt> logger,
            INetworkManager networkManagerInit,
            IServer serverInit,
            IRpcServer rpcInit,
            IBinarySerializer serializer,
            IBlockchain blockchain,
            IWalletManager walletManager,
            ICryptoInitializer cryptoInitializer,
            IBinaryInitializer binaryInitializer)
        {
            _consoleReader = consoleReaderInit;
            _consoleWriter = consoleWriterInit;
            _logger = logger;
            _networkManager = networkManagerInit;
            _server = serverInit;
            _serializer = serializer;
            _rpc = rpcInit;
            _blockchain = blockchain;
            _loggerFactory = loggerFactory;
            _logs = new ConcurrentBag<LogEntry>();
            _walletManager = walletManager;
        }

        /// <inheritdoc />
        public void StartPrompt(string[] args)
        {
            _logger.LogInformation("Starting Prompt");
            _consoleWriter.WriteLine("Neo-Sharp", ConsoleOutputStyle.Prompt);

            if (args != null)
            {
                // Append arguments as inputs

                _consoleReader.AppendInputs(args);
            }

            _blockchain.InitializeBlockchain().Wait();

            while (!_exit)
            {
                // Read log buffer

                while (_logs.TryTake(out var log))
                {
                    _consoleWriter.WriteLine
                        (
                        "[" + log.Level + (string.IsNullOrEmpty(log.Category) ? "" : "-" + log.Category) + "] " +
                        log.MessageWithError, _logStyle[log.Level]
                        );
                }

                // Read input

                var fullCmd = _consoleReader.ReadFromConsole(_commandAutocompleteCache);

                if (string.IsNullOrWhiteSpace(fullCmd))
                {
                    continue;
                }

                _logger.LogInformation("Execute: " + fullCmd);

                Execute(fullCmd);
            }

            _consoleWriter.WriteLine("Exiting", ConsoleOutputStyle.Information);
        }

        private static IEnumerable<PromptCommandAttribute> SearchCommands(string command, List<CommandToken> cmdArgs)
        {
            // Parse arguments

            cmdArgs.AddRange(command.SplitCommandLine());
            if (cmdArgs.Count <= 0) yield break;

            foreach (var key in _commandCache)
            {
                if (key.Key.Length > cmdArgs.Count) continue;

                var equal = true;
                for (int x = 0, m = key.Key.Length; x < m; x++)
                {
                    var c = cmdArgs[x];
                    if (!string.Equals(c.Value, key.Key[x], StringComparison.InvariantCultureIgnoreCase))
                    {
                        equal = false;
                        break;
                    }
                }

                if (equal)
                {
                    yield return key.Value;
                }
            }
        }

        private static PromptCommandAttribute SearchRightCommand(IReadOnlyList<PromptCommandAttribute> cmds, IEnumerable<CommandToken> args)
        {
            switch (cmds.Count)
            {
                case 0: return null;
                case 1: return cmds[0];
                default:
                    {
                        // Multiple commands

                        PromptCommandAttribute cmd = null;

                        foreach (var a in cmds)
                        {
                            try
                            {
                                a.ConvertToArguments(args.Skip(a.CommandLength).ToArray());

                                if (cmd == null || cmd.Order > a.Order)
                                    cmd = a;
                            }
                            catch { }
                        }

                        return cmd;
                    }
            }
        }

        /// <inheritdoc />
        public bool Execute(string command)
        {
            command = command.Trim();
            PromptCommandAttribute[] cmds = null;

            try
            {
                // Parse arguments

                var cmdArgs = new List<CommandToken>();
                cmds = SearchCommands(command, cmdArgs).ToArray();
                var cmd = SearchRightCommand(cmds, cmdArgs);

                if (cmd == null)
                {
                    if (cmds.Length > 0)
                    {
                        throw (new Exception($"Wrong parameters for <{cmds.FirstOrDefault().Command}>"));
                    }

                    throw (new Exception($"Command not found <{command}>"));
                }

                // Get command

                lock (_consoleReader) lock (_consoleWriter)
                    {
                        // Raise event

                        OnCommandRequested?.Invoke(this, cmd, command);

                        // Invoke

                        var ret = cmd.Method.Invoke(this, cmd.ConvertToArguments(cmdArgs.Skip(cmd.CommandLength).ToArray()));

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
                _consoleWriter.WriteLine(msg, ConsoleOutputStyle.Error);

                PrintHelp(cmds);
                return false;
            }
        }
    }
}