using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NeoSharp.Application.Attributes;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;
using NeoSharp.VM;
using NeoSharp.VM.Types;

namespace NeoSharp.Application.Client
{
    public partial class Prompt : IPrompt
    {
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

                if (contract == null /* TODO: || (isDynamicInvoke && !contract.AllowDynamicInvokes) */) return null;

                return contract.Script;
            }
        }

        DebugScriptTable _scriptTable = new DebugScriptTable();

        // TODO: attach
        // testinvoke {contract hash} {params} (--attach-neo={amount}, --attach-gas={amount}) (--from-addr={addr})

        /// <summary>
        /// Add virtual contract
        /// </summary>
        /// <param name="script">Script</param>
        [PromptCommand("virtual contract add", Help = "Add virtual contract to the script table", Category = "Debug")]
        private void VirtualContractAddCommand(byte[] script)
        {
            var hash = new UInt160(Crypto.Default.Hash160(script));

            _scriptTable.VirtualContracts.TryAdd(hash, script);

            foreach (var h in _scriptTable.VirtualContracts.Keys)
            {
                _consoleWriter.WriteLine(h.ToString(true), h == hash ? ConsoleOutputStyle.Information : ConsoleOutputStyle.Output);
            }
        }

        /// <summary>
        /// Add virtual contract
        /// </summary>
        /// <param name="file">File</param>
        [PromptCommand("virtual contract add", Help = "Add virtual contract to the script table", Category = "Debug")]
        private void VirtualContractAddCommand(FileInfo file)
        {
            if (!file.Exists) throw new ArgumentException("File must exists");

            VirtualContractAddCommand(File.ReadAllBytes(file.FullName));
        }

        /// <summary>
        /// Clear virtual contract
        /// </summary>
        [PromptCommand("virtual contract clear", Help = "Clear all virtual smart contracts from the script table", Category = "Debug")]
        private void VirtualContractClearCommand()
        {
            _scriptTable.VirtualContracts.Clear();
        }

        /// <summary>
        /// List virtual contract
        /// </summary>
        [PromptCommand("virtual contract list", Help = "List all virtual smart contracts on the script table", Category = "Debug")]
        private void VirtualContractListCommand()
        {
            foreach (var h in _scriptTable.VirtualContracts.Keys)
            {
                _consoleWriter.WriteLine(h.ToString(true));
            }
        }

        /// <summary>
        /// Decompile
        /// </summary>
        /// <param name="contractHash">Contract</param>
        [PromptCommand("decompile", Help = "Decompile contract", Category = "Debug")]
        private void DecompileCommand(UInt160 contractHash)
        {
            var script = _scriptTable.GetScript(contractHash.ToArray(), false);

            if (script == null) throw (new ArgumentNullException("Contract not found"));

            var parser = new InstructionParser();
            foreach (var i in parser.Parse(script))
            {
                _consoleWriter.Write(i.Location.ToString() + " ", ConsoleOutputStyle.Information);

                if (i is InstructionWithPayload ip)
                {
                    _consoleWriter.Write(i.OpCode.ToString() + " ");
                    _consoleWriter.WriteLine("{" + ip.Payload.ToHexString(true) + "}", ConsoleOutputStyle.DarkGray);
                }
                else
                {
                    _consoleWriter.WriteLine(i.OpCode.ToString());
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
        private void TestInvoke(UInt160 contractHash, ETriggerType trigger, string operation, [PromptCommandParameterBody] object[] parameters = null)
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
                var result = new { vm.State, Result = vm.ResultStack, vm.ConsumedGas };

                _consoleWriter.WriteObject(result, PromptOutputStyle.json);
            }

            //_logger.LogDebug("Execution opcodes:" + Environment.NewLine + log.ToString());
        }
    }
}