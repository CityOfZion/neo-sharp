using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using NeoSharp.Application.Attributes;
using NeoSharp.Application.Client;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Models;
using NeoSharp.Cryptography;
using NeoSharp.Types;
using NeoSharp.Types.ExtensionMethods;
using NeoSharp.VM;
using NeoSharp.VM.Types;

namespace NeoSharp.Application.Controllers
{
    public class PromptDebugController : IPromptController
    {
        #region Private fields

        private readonly ILogBag _logs;
        private readonly Core.Logging.ILogger<Prompt> _logger;
        private readonly ILoggerFactoryExtended _log;

        private readonly IVMFactory _vmFactory;
        private readonly IConsoleHandler _consoleHandler;

        #endregion

        [Flags]
        public enum LogVerbose : byte
        {
            Off = 0,

            Trace = 1,
            Debug = 2,
            Information = 4,
            Warning = 8,
            Error = 16,
            Critical = 32,

            All = Trace | Debug | Information | Warning | Error | Critical
        }

        private readonly Dictionary<LogLevel, LogVerbose> _logFlagProxy = new Dictionary<LogLevel, LogVerbose>()
        {
            { LogLevel.Trace, LogVerbose.Trace},
            { LogLevel.Debug, LogVerbose.Debug},
            { LogLevel.Information, LogVerbose.Information},
            { LogLevel.Warning, LogVerbose.Warning},
            { LogLevel.Error, LogVerbose.Error},
            { LogLevel.Critical, LogVerbose.Critical},
        };
        private LogVerbose _logVerbose = LogVerbose.Off;

        private void Log_OnLog(LogEntry log)
        {
            if (!_logVerbose.HasFlag(_logFlagProxy[log.Level]))
            {
                return;
            }

            _logs.Add(log);
        }

        /// <summary>
        /// Enable / Disable logs
        /// </summary>
        /// <param name="mode">Mode</param>
        [PromptCommand("log", Help = "Enable/Disable log output", Category = "Usability")]
        public void LogCommand(LogVerbose mode)
        {
            _logVerbose = mode;

            if (mode != LogVerbose.Off)
            {
                _log.OnLog -= Log_OnLog;
                _log.OnLog += Log_OnLog;

                _logger.LogDebug("Log output is enabled");
            }
            else
            {
                _logs.Clear();
                _logger.LogDebug("Log output is disabled");

                _log.OnLog -= Log_OnLog;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logBag">Log bag</param>
        /// <param name="logger">Logger</param>
        /// <param name="log">Log</param>
        /// <param name="vmFactory">VM Factory</param>
        /// <param name="consoleHandler">Console handler</param>
        public PromptDebugController
            (
            ILogBag logBag,
            Core.Logging.ILogger<Prompt> logger, ILoggerFactoryExtended log,
            IVMFactory vmFactory, IConsoleHandler consoleHandler
            )
        {
            _logs = logBag;
            _log = log;
            _logger = logger;
            _vmFactory = vmFactory;
            _consoleHandler = consoleHandler;
        }

        class DebugScriptTable : IScriptTable
        {
            public readonly Dictionary<UInt160, byte[]> VirtualContracts = new Dictionary<UInt160, byte[]>();

            /// <summary>
            /// Get script table
            /// </summary>
            /// <param name="scriptHash">Script hash</param>
            /// <param name="isDynamicInvoke">Is dynamic invoke</param>
            /// <returns>return smart contract</returns>
            public byte[] GetScript(byte[] scriptHash, bool isDynamicInvoke)
            {
                var hash = new UInt160(scriptHash);

                if (VirtualContracts.TryGetValue(hash, out var script))
                {
                    return script;
                }

                Contract contract = Contract.GetContract(hash);

                if (contract == null /* TODO #400: || (isDynamicInvoke && !contract.AllowDynamicInvokes) */) return null;

                return contract.Script;
            }
        }

        DebugScriptTable _scriptTable = new DebugScriptTable();

        // TODO #401: Implement test invoke with asset attachment
        // testinvoke {contract hash} {params} (--attach-neo={amount}, --attach-gas={amount}) (--from-addr={addr})

        /// <summary>
        /// Add virtual contract
        /// </summary>
        /// <param name="script">Script</param>
        [PromptCommand("virtual contract add", Help = "Add virtual contract to the script table", Category = "Debug")]
        public void VirtualContractAddCommand(byte[] script)
        {
            var hash = new UInt160(Crypto.Default.Hash160(script));

            _scriptTable.VirtualContracts.TryAdd(hash, script);

            foreach (var h in _scriptTable.VirtualContracts.Keys)
            {
                _consoleHandler.WriteLine(h.ToString(true), h == hash ? ConsoleOutputStyle.Information : ConsoleOutputStyle.Output);
            }
        }

        /// <summary>
        /// Add virtual contract
        /// </summary>
        /// <param name="file">File</param>
        [PromptCommand("virtual contract add", Help = "Add virtual contract to the script table", Category = "Debug")]
        public void VirtualContractAddCommand(FileInfo file)
        {
            if (!file.Exists) throw new ArgumentException("File must exists");

            VirtualContractAddCommand(File.ReadAllBytes(file.FullName));
        }

        /// <summary>
        /// Clear virtual contract
        /// </summary>
        [PromptCommand("virtual contract clear", Help = "Clear all virtual smart contracts from the script table", Category = "Debug")]
        public void VirtualContractClearCommand()
        {
            _scriptTable.VirtualContracts.Clear();
        }

        /// <summary>
        /// List virtual contract
        /// </summary>
        [PromptCommand("virtual contract list", Help = "List all virtual smart contracts on the script table", Category = "Debug")]
        public void VirtualContractListCommand()
        {
            foreach (var h in _scriptTable.VirtualContracts.Keys)
            {
                _consoleHandler.WriteLine(h.ToString(true));
            }
        }

        /// <summary>
        /// Decompile
        /// </summary>
        /// <param name="contractHash">Contract</param>
        [PromptCommand("decompile", Help = "Decompile contract", Category = "Debug")]
        public void DecompileCommand(UInt160 contractHash)
        {
            var script = _scriptTable.GetScript(contractHash.ToArray(), false);

            if (script == null) throw (new ArgumentNullException("Contract not found"));

            var parser = new InstructionParser();
            foreach (var i in parser.Parse(script))
            {
                _consoleHandler.Write(i.Location.ToString() + " ", ConsoleOutputStyle.Information);

                if (i is InstructionWithPayload ip)
                {
                    _consoleHandler.Write(i.OpCode.ToString() + " ");
                    _consoleHandler.WriteLine("{" + ip.Payload.ToHexString(true) + "}", ConsoleOutputStyle.DarkGray);
                }
                else
                {
                    _consoleHandler.WriteLine(i.OpCode.ToString());
                }
            }
        }

        /// <summary>
        /// Invoke contract
        /// </summary>
        /// <param name="contractHash">Contract</param>
        /// <param name="trigger">Trigger</param>
        /// <param name="operation">Operation</param>
        /// <param name="parameters">Parameters</param>
        [PromptCommand("testinvoke", Help = "Test invoke contract", Category = "Debug")]
        public void TestInvoke(UInt160 contractHash, ETriggerType trigger, string operation, [PromptCommandParameterBody] object[] parameters = null)
        {
            if (_scriptTable.GetScript(contractHash.ToArray(), false) == null) throw (new ArgumentNullException("Contract not found"));

            var args = new ExecutionEngineArgs()
            {
                ScriptTable = _scriptTable,
                Logger = new ExecutionEngineLogger(ELogVerbosity.StepInto),
                Trigger = trigger
            };

            var log = new StringBuilder();

            args.Logger.OnStepInto += (context) =>
            {
                log.AppendLine(context.NextInstruction.ToString());
            };

            using (var script = new ScriptBuilder())
            using (var vm = _vmFactory.Create(args))
            {
                script.EmitMainPush(operation, parameters);
                script.EmitAppCall(contractHash.ToArray(), false);

                vm.LoadScript(script);

                var ret = vm.Execute();
                var result = new
                {
                    vm.State,
                    Result = vm.ResultStack,
                    vm.ConsumedGas
                };

                _consoleHandler.WriteObject(result, PromptOutputStyle.json);
            }

            //_logger.LogDebug("Execution opcodes:" + Environment.NewLine + log.ToString());
        }
    }
}