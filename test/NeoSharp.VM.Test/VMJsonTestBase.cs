using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.VM;
using NeoSharp.Types.ExtensionMethods;
using NeoSharp.VM.Extensions;
using NeoSharp.VM.TestHelper;
using NeoSharp.VM.TestHelper.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeoSharp.VM.Test
{
    public abstract class VMJsonTestBase
    {
        /// <summary>
        /// Execute this test
        /// </summary>
        /// <param name="factory">Factory</param>
        /// <param name="json">Json</param>
        public void ExecuteTest(IVMFactory factory, string json) => ExecuteTest(factory, json.JsonToVMUT());

        /// <summary>
        /// Execute this test
        /// </summary>
        /// <param name="factory">Factory</param>
        /// <param name="ut">Test</param>
        public void ExecuteTest(IVMFactory factory, VMUT ut)
        {
            foreach (var test in ut.Tests)
            {
                // Arguments

                var storages = new ManualStorage();
                var interopService = new InteropService();
                var contracts = new ManualContracts();
                var state = new StateMachine
                (
                    null, null, null, contracts,
                    storages, interopService, null, null, null, null
                );

                var args = new ExecutionEngineArgs
                {
                    Trigger = test.Trigger,
                    InteropService = interopService,
                    Logger = new ExecutionEngineLogger(ELogVerbosity.StepInto),
                    ScriptTable = contracts,
                };

                var log = new StringBuilder();
                var logBag = new List<string>();
                var notBag = new List<JToken>();

                interopService.OnLog += (sender, e) => logBag.Add(e.Message);
                interopService.OnNotify += (sender, e) => notBag.Add(ItemToJson(e.State));
                args.Logger.OnStepInto += (ctx) => log.AppendLine(ctx.InstructionPointer.ToString("x6") + " - " + ctx.NextInstruction);
                interopService.OnSysCall += (o, e) =>
                {
                    // Remove last line
                    log.Remove(log.Length - Environment.NewLine.Length, Environment.NewLine.Length);
                    log.AppendLine($" [{e.MethodName}  -  {e.Result}]");
                };

                // Message provider

                if (test.Message != null)
                {
                    args.MessageProvider = new ManualMessageProvider(test.Message, test.Message);
                }


                // Script table

                if (test.ScriptTable != null)
                {
                    foreach (var script in test.ScriptTable)
                    {
                        var contract = new Contract
                        {
                            Code = new Code
                            {
                                Script = script.Script,
                                ScriptHash = script.Script.ToScriptHash(),
                                Metadata = ContractMetadata.NoProperty
                            }
                        };

                        if (script.HasDynamicInvoke) contract.Code.Metadata |= ContractMetadata.HasDynamicInvoke;
                        if (script.HasStorage) contract.Code.Metadata |= ContractMetadata.HasStorage;
                        if (script.Payable) contract.Code.Metadata |= ContractMetadata.Payable;

                        contracts.Add(contract.ScriptHash, contract);
                    }

                    contracts.Commit();
                }

                // Create engine

                using (var engine = factory.Create(args))
                {
                    engine.GasAmount = ulong.MaxValue;
                    engine.LoadScript(test.Script);

                    // Execute Steps

                    if (test.Steps != null)
                    {
                        foreach (var step in test.Steps)
                        {
                            // Actions

                            log.Clear();
                            logBag.Clear();
                            notBag.Clear();

                            if (step.Actions != null) foreach (var run in step.Actions)
                                {
                                    switch (run)
                                    {
                                        case VMUTActionType.Execute: engine.Execute(); break;
                                        case VMUTActionType.StepInto: engine.StepInto(); break;
                                        case VMUTActionType.StepOut: engine.StepOut(); break;
                                        case VMUTActionType.StepOver: engine.StepOver(); break;
                                        case VMUTActionType.Clean: engine.Clean(); break;
                                    }
                                }

                            // Review results

                            var add = string.IsNullOrEmpty(step.Name) ? "" : "-" + step.Name;

                            AssertResult(engine, step.State, logBag, notBag, $"{ut.Category}-{ut.Name}{add}: ");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Assert result
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="result">Result</param>
        /// <param name="logBag">Log bag</param>
        /// <param name="notBag">Not bag</param>
        /// <param name="message">Message</param>
        private void AssertResult(ExecutionEngineBase engine, VMUTExecutionEngineState result, List<string> logBag, List<JToken> notBag, string message)
        {
            AssertAreEqual(engine.State.ToString(), result.State.ToString(), message + "State is different");
            AssertAreEqual(engine.ConsumedGas, result.ConsumedGas, message + "Consumed gas is different");

            AssertAreEqual(logBag.ToArray(), result.Logs ?? new string[0], message + "Logs are different");
            AssertAreEqual(notBag.ToArray(), result.Notifications == null ? new JToken[0] : result.Notifications.Select(u => PrepareJsonItem(u)).ToArray(), message + "Notifies are different");

            AssertResult(engine.InvocationStack, result.InvocationStack, message + " [Invocation stack]");
            AssertResult(engine.ResultStack, result.ResultStack, message + " [Result stack] ");
        }

        /// <summary>
        /// Assert invocation stack
        /// </summary>
        /// <param name="stack">Stack</param>
        /// <param name="result">Result</param>
        /// <param name="message">Message</param>
        private void AssertResult(StackBase<ExecutionContextBase> stack, VMUTExecutionContextState[] result, string message)
        {
            AssertAreEqual(stack.Count, result == null ? 0 : result.Length, message + "Stack is different");

            for (int x = 0, max = stack.Count; x < max; x++)
            {
                var context = stack.Peek(x);

                AssertAreEqual("0x" + context.ScriptHash.ToHexString(false).ToUpper(), "0x" + result[x].ScriptHash.ToHexString(false).ToUpper(), message + "Script hash is different");
                AssertAreEqual(context.NextInstruction, result[x].NextInstruction, message + "Next instruction is different");
                AssertAreEqual(context.InstructionPointer, result[x].InstructionPointer, message + "Instruction pointer is different");

                AssertResult(context.EvaluationStack, result[x].EvaluationStack, message + " [EvaluationStack]");
                AssertResult(context.AltStack, result[x].AltStack, message + " [AltStack]");
            }
        }

        /// <summary>
        /// Assert result stack
        /// </summary>
        /// <param name="stack">Stack</param>
        /// <param name="result">Result</param>
        /// <param name="message">Message</param>
        private void AssertResult(Stack stack, VMUTStackItem[] result, string message)
        {
            AssertAreEqual(stack.Count, result == null ? 0 : result.Length, message + "Stack is different");

            for (int x = 0, max = stack.Count; x < max; x++)
            {
                AssertAreEqual(ItemToJson(stack.Peek(x)).ToString(Formatting.None), PrepareJsonItem(result[x]).ToString(Formatting.None), message + "Stack item is different");
            }
        }

        private JObject PrepareJsonItem(VMUTStackItem item)
        {
            var ret = new JObject
            {
                ["type"] = item.Type.ToString(),
                ["value"] = item.Value
            };

            switch (item.Type)
            {
                case VMUTStackItemType.String:
                    {
                        // Easy access

                        ret["type"] = VMUTStackItemType.ByteArray.ToString();
                        ret["value"] = Encoding.UTF8.GetBytes(item.Value.Value<string>());
                        break;
                    }
                case VMUTStackItemType.ByteArray:
                    {
                        var value = ret["value"].Value<string>();

                        if (value.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ret["value"] = value.FromHexString();
                        }
                        else
                        {
                            ret["value"] = Convert.FromBase64String(value);
                        }

                        break;
                    }
                case VMUTStackItemType.Integer:
                    {
                        // Ensure format

                        ret["value"] = ret["value"].Value<string>();
                        break;
                    }
                case VMUTStackItemType.Struct:
                case VMUTStackItemType.Array:
                    {
                        var array = (JArray)ret["value"];

                        for (int x = 0, m = array.Count; x < m; x++)
                        {
                            array[x] = PrepareJsonItem(JsonConvert.DeserializeObject<VMUTStackItem>(array[x].ToString()));
                        }

                        ret["value"] = array;
                        break;
                    }
                case VMUTStackItemType.Map:
                    {
                        var obj = (JObject)ret["value"];

                        foreach (var prop in obj.Properties())
                        {
                            obj[prop.Name] = PrepareJsonItem(JsonConvert.DeserializeObject<VMUTStackItem>(prop.Value.ToString()));
                        }

                        ret["value"] = obj;
                        break;
                    }
            }

            return ret;
        }

        private JToken ItemToJson(StackItemBase item)
        {
            if (item == null) return null;

            JToken value;

            using (item)
            {
                switch (item.Type)
                {
                    case EStackItemType.Bool: value = new JValue((bool)item.ToObject()); break;
                    case EStackItemType.Integer: value = new JValue(item.ToObject().ToString()); break;
                    case EStackItemType.ByteArray: value = new JValue((byte[])item.ToObject()); break;
                    case EStackItemType.Struct:
                    case EStackItemType.Array:
                        {
                            var array = (ArrayStackItemBase)item;
                            var jarray = new JArray();

                            foreach (var entry in array)
                            {
                                jarray.Add(ItemToJson(entry));
                            }

                            value = jarray;
                            break;
                        }
                    case EStackItemType.Map:
                        {
                            var dic = (MapStackItemBase)item;
                            var jdic = new JObject();

                            foreach (var entry in dic)
                            {
                                jdic.Add(entry.Key.ToByteArray().ToHexString(true), ItemToJson(entry.Value));
                            }

                            value = jdic;
                            break;
                        }
                    case EStackItemType.Interop:
                        {
                            var obj = item.ToObject();

                            if (obj is IMessageProvider) value = "IMessageProvider";
                            else if (obj is StorageContext) value = "StorageContext";
                            else throw new NotImplementedException();

                            break;
                        }
                    default: throw new NotImplementedException();
                }

                return new JObject
                {
                    ["type"] = item.Type.ToString(),
                    ["value"] = value
                };
            }
        }

        /// <summary>
        /// Assert with message
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <param name="message">Message</param>
        private void AssertAreEqual(object a, object b, string message)
        {
            if (a is byte[] ba) a = ba.ToHexString().ToUpperInvariant();
            if (b is byte[] bb) b = bb.ToHexString().ToUpperInvariant();

            if (a is IList ca && b is IList cb)
            {
                a = a.ToJson();
                b = b.ToJson();
            }

            Assert.AreEqual(a, b, message +
                $"{Environment.NewLine}Expected:{Environment.NewLine + a.ToString() + Environment.NewLine}Actual:{Environment.NewLine + b.ToString()}");
        }
    }
}