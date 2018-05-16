using Microsoft.Extensions.Logging;
using NeoSharp.Application.Attributes;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;
using NeoSharp.Core.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NeoSharp.Application.Client
{
    public class Prompt : IPrompt
    {
        #region Variables

        /// <summary>
        /// Exit flag
        /// </summary>
        private bool _exit;
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
        /// Rpc server
        /// </summary>
        private readonly IRpcServer _rpc;
        /// <summary>
        /// Command cache
        /// </summary>
        private static readonly IDictionary<string[], PromptCommandAttribute> _commandCache;
        private static readonly IDictionary<string, List<ParameterInfo[]>> _commandAutocompleteCache;

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
        public Prompt(IConsoleReader consoleReaderInit, IConsoleWriter consoleWriterInit,
            ILogger<Prompt> logger, INetworkManager networkManagerInit,
            IServer serverInit, IRpcServer rpcInit)
        {
            _consoleReader = consoleReaderInit;
            _consoleWriter = consoleWriterInit;
            _logger = logger;
            _networkManager = networkManagerInit;
            _server = serverInit;
            _rpc = rpcInit;
        }

        public void StartPrompt(string[] args)
        {
            _logger.LogInformation("Starting Prompt");
            _consoleWriter.WriteLine("Neo-Sharp");

            while (!_exit)
            {
                var fullCmd = _consoleReader.ReadFromConsole(_commandAutocompleteCache);
                if (string.IsNullOrWhiteSpace(fullCmd)) continue;

                Execute(fullCmd);
            }

            _consoleWriter.WriteLine("Exiting", ConsoleOutputStyle.Information);
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return false if fail</returns>
        public bool Execute(string command)
        {
            PromptCommandAttribute cmd = null;
            List<PromptCommandAttribute> cmds = new List<PromptCommandAttribute>();

            try
            {
                // Parse arguments

                var cmdArgs = new List<CommandToken>(command.SplitCommandLine());
                if (cmdArgs.Count <= 0) return false;

                // Search command

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
                        cmds.Add(key.Value);
                    }
                }

                switch (cmds.Count)
                {
                    case 0: throw (new Exception($"Command not found <{command}>"));
                    case 1: cmd = cmds[0]; break;
                    case 2:
                        {
                            // Multiple commands

                            foreach (var a in cmds)
                            {
                                try
                                {
                                    a.ConvertToArguments(cmdArgs.Skip(a.CommandLength).ToArray());
                                    cmd = a;
                                }
                                catch { }
                            }

                            if (cmd == null)
                            {
                                cmd = cmds[0]; // for help message
                                goto case 0;
                            }

                            break;
                        }
                }

                // Get command

                lock (_consoleReader) lock (_consoleWriter)
                        cmd.Method.Invoke(this, cmd.ConvertToArguments(cmdArgs.Skip(cmd.CommandLength).ToArray()));

                return true;
            }
            catch (Exception e)
            {
                _consoleWriter.WriteLine(e.Message, ConsoleOutputStyle.Error);

                // Print help

                if (cmd != null && !string.IsNullOrEmpty(cmd.Help))
                {
                    _consoleWriter.WriteLine(cmd.Help, ConsoleOutputStyle.Information);

                    if (cmds.Count > 0)
                    {
                        _consoleWriter.WriteLine("Examples:", ConsoleOutputStyle.Information);

                        // How to use?

                        foreach (var v in cmds)
                        {
                            string args = "";

                            if (v.Parameters != null && v.Parameters.Length > 0)
                            {
                                foreach (var par in v.Parameters)
                                    args += $" <{par.Name}>";
                            }

                            _consoleWriter.WriteLine("  " + v.Command + args, ConsoleOutputStyle.Information);
                        }
                    }
                }

                return false;
            }
        }

        #region Commands

        #region Invokes

        /// <summary>
        /// Invoke contract
        /// </summary>
        /// <param name="contractHash">Contract</param>
        /// <param name="body">Body</param>
        [PromptCommand("invoke", Help = "Invoke a contract", Category = "Invokes")]
        private void Invoke(UInt160 contractHash, [PromptCommandParameterBody] object[] args)
        {
            Contract contract = Contract.GetContract(contractHash);
            if (contract == null) throw (new ArgumentNullException("Contract not found"));

            var tx = contract.CreateInvokeTransaction(args);
        }

        /// <summary>
        /// Invoke contract
        /// </summary>
        /// <param name="contractHash">Contract</param>
        /// <param name="body">Body</param>
        [PromptCommand("testinvoke", Help = "Test invoke contract", Category = "Invokes")]
        private void TestInvoke(UInt160 contractHash, [PromptCommandParameterBody] object[] args)
        {
            Contract contract = Contract.GetContract(contractHash);
            if (contract == null) throw (new ArgumentNullException("Contract not found"));

            var tx = contract.CreateInvokeTransaction(args);
        }

        #endregion

        #region Network

        /// <summary>
        /// Nodes
        /// </summary>
        [PromptCommand("nodes", Category = "Network", Help = "Get nodes information")]
        // ReSharper disable once UnusedMember.Local
        private void NodesCommand()
        {
            lock (_server.ConnectedPeers)
            {
                IPeer[] peers = _server.ConnectedPeers.ToArray();

                _consoleWriter.WriteLine("Connected: " + peers.Length.ToString());

                foreach (IPeer p in peers)
                {
                    _consoleWriter.WriteLine(p.ToString());
                }
            }
        }

        /// <summary>
        /// Start rpc
        /// </summary>
        [PromptCommand("start rpc", Category = "Network")]
        private void StartRpcCommand()
        {
            _rpc?.Start();
        }

        /// <summary>
        /// Stop rpc
        /// </summary>
        [PromptCommand("stop rpc", Category = "Network")]
        private void StopRpcCommand()
        {
            _rpc?.Stop();
        }

        /// <summary>
        /// Start network
        /// </summary>
        [PromptCommand("start network", Category = "Network")]
        // ReSharper disable once UnusedMember.Local
        private void StartCommand()
        {
            _networkManager?.StartNetwork();
        }

        /// <summary>
        /// Stop network
        /// </summary>
        [PromptCommand("stop network", Category = "Network")]
        private void StopCommand()
        {
            _networkManager?.StopNetwork();
        }

        #endregion

        #region Wallet

        [PromptCommand("create wallet", Category = "Wallet", Help = "Create a new wallet")]
        private void CreateWalletCommand(FileInfo file)
        {
            if (file.Exists)
            {
                _consoleWriter.WriteLine($"File '{file.FullName}' already exist, please provide a new one", ConsoleOutputStyle.Error);
                return;
            }
        }

        [PromptCommand("open wallet", Category = "Wallet", Help = "Open wallet")]
        private void OpenWalletCommand(FileInfo file)
        {
            if (!file.Exists)
            {
                _consoleWriter.WriteLine($"File not found '{file.FullName}'", ConsoleOutputStyle.Error);
                return;
            }
        }

        #endregion

        #region Blockchain

        /// <summary>
        /// Show state
        /// </summary>
        [PromptCommand("state", Category = "Blockchain", Help = "Show current state")]
        private void StateCommand()
        {

        }

        /// <summary>
        /// Get block by index
        /// </summary>
        /// <param name="index">Index</param>
        [PromptCommand("block", Category = "Blockchain", Help = "Get block by index or by hash")]
        private void BlockCommand(ulong index)
        {
            // TODO: Change this

            Block block = Blockchain.GenesisBlock;
            _consoleWriter.WriteLine(JsonConvert.SerializeObject(block, Formatting.Indented));
        }

        /// <summary>
        /// Get block by hash
        /// </summary>
        /// <param name="hash">Hash</param>
        [PromptCommand("block", Category = "Blockchain", Help = "Get block by index or by hash")]
        private void BlockCommand(UInt256 hash)
        {
            // TODO: Change this

            Block block = Blockchain.GenesisBlock;
            _consoleWriter.WriteLine(JsonConvert.SerializeObject(block, Formatting.Indented));
        }

        /// <summary>
        /// Get tx by hash
        /// </summary>
        /// <param name="hash">Hash</param>
        [PromptCommand("tx", Category = "Blockchain", Help = "Get tx")]
        private void TxCommand(UInt256 hash)
        {
            // TODO: Change this

            Transaction tx = new Transaction();
            _consoleWriter.WriteLine(tx.ToJson(true));
        }

        #endregion

        #region Usability

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
        // ReSharper disable once UnusedMember.Local
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
        [PromptCommand("exit", Category = "Usability")]
        // ReSharper disable once UnusedMember.Local
        private void ExitCommand()
        {
            StopCommand();
            _exit = true;
        }

        /// <summary>
        /// Show help
        /// </summary>
        [PromptCommand("help", Category = "Usability")]
        // ReSharper disable once UnusedMember.Local
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

        #endregion

        #endregion
    }
}