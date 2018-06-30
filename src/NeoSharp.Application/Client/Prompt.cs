using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NeoSharp.Application.Attributes;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Network;
using NeoSharp.Core.Network.Rpc;
using NeoSharp.Core.Types;
using NeoSharp.Core.Wallet;
using Newtonsoft.Json;

namespace NeoSharp.Application.Client
{
    public partial class Prompt : IPrompt
    {
        #region Variables

        public enum PromptOutputStyle { json, raw };

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
        private readonly ILogger<Prompt> _logger;
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

        public delegate void delOnCommandRequested(IPrompt prompt, PromptCommandAttribute cmd, string commandLine);
        public event delOnCommandRequested OnCommandRequested;

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
        /// <param name="logger">Logger</param>
        /// <param name="networkManagerInit">Network manger init</param>
        /// <param name="serverInit">Server</param>
        /// <param name="rpcInit">Rpc server</param>
        /// <param name="serializer">Binary serializer</param>
        /// <param name="blockchain">Blockchain</param>
        public Prompt(IConsoleReader consoleReaderInit, IConsoleWriter consoleWriterInit,
            ILogger<Prompt> logger, INetworkManager networkManagerInit,
                      IServer serverInit, IRpcServer rpcInit, IBinarySerializer serializer, IBlockchain blockchain, IWalletManager walletManager)
        {
            _consoleReader = consoleReaderInit;
            _consoleWriter = consoleWriterInit;
            _logger = logger;
            _networkManager = networkManagerInit;
            _server = serverInit;
            _serializer = serializer;
            _rpc = rpcInit;
            _blockchain = blockchain;
            _walletManager = walletManager;
        }

        public void StartPrompt(string[] args)
        {
            _logger.LogInformation("Starting Prompt");
            _consoleWriter.WriteLine("Neo-Sharp");

            if (args != null)
            {
                // Append arguments as inputs

                _consoleReader.AppendInputs(args);
            }

            while (!_exit)
            {
                var fullCmd = _consoleReader.ReadFromConsole(_commandAutocompleteCache);
                if (string.IsNullOrWhiteSpace(fullCmd)) continue;

                Execute(fullCmd);
            }

            _consoleWriter.WriteLine("Exiting", ConsoleOutputStyle.Information);
        }

        IEnumerable<PromptCommandAttribute> SearchCommands(string command, List<CommandToken> cmdArgs)
        {
            // Parse arguments

            cmdArgs.AddRange(command.SplitCommandLine());
            if (cmdArgs.Count <= 0) yield break;

            foreach (KeyValuePair<string[], PromptCommandAttribute> key in _commandCache)
            {
                if (key.Key.Length > cmdArgs.Count) continue;

                bool equal = true;
                for (int x = 0, m = key.Key.Length; x < m; x++)
                {
                    CommandToken c = cmdArgs[x];
                    if (c.Value.ToLowerInvariant() != key.Key[x])
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

        PromptCommandAttribute SearchRightCommand(PromptCommandAttribute[] cmds, IEnumerable<CommandToken> args)
        {
            switch (cmds.Length)
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

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return false if fail</returns>
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
                        throw (new Exception($"Wrong parameters for <{cmds.FirstOrDefault().Command}>"));

                    throw (new Exception($"Command not found <{command}>"));
                }

                // Get command

                lock (_consoleReader) lock (_consoleWriter)
                    {
                        // Raise event

                        OnCommandRequested?.Invoke(this, cmd, command);

                        // Invoke

                        cmd.Method.Invoke(this, cmd.ConvertToArguments(cmdArgs.Skip(cmd.CommandLength).ToArray()));
                    }

                return true;
            }
            catch (Exception e)
            {
                string msg = e.InnerException != null ? e.InnerException.Message : e.Message;
                _consoleWriter.WriteLine(msg, ConsoleOutputStyle.Error);

                PrintHelp(cmds);
                return false;
            }
        }

        /// <summary>
        /// Write object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="output">Output</param>
        private void WriteObject(object obj, PromptOutputStyle output)
        {
            switch (output)
            {
                case PromptOutputStyle.json:
                    {
                        _consoleWriter.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
                        break;
                    }
                case PromptOutputStyle.raw:
                    {
                        if (obj is byte[] data)
                            _consoleWriter.WriteLine(data.ToHexString(true));
                        else
                            _consoleWriter.WriteLine(_serializer.Serialize(obj).ToHexString(true));

                        break;
                    }
            }
        }
    }
}