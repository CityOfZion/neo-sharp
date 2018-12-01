using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.VM.TestHelper;
using NeoSharp.VM.TestHelper.Extensions;

namespace NeoSharp.VM.Test
{
    public abstract class VMJsonTestBase
    {
        /// <summary>
        /// Execute this test
        /// </summary>
        /// <param name="json">Json</param>
        public void ExecuteTest(string json) => ExecuteTest(json.JsonToVMUT());

        /// <summary>
        /// Execute this test
        /// </summary>
        /// <param name="ut">Test</param>
        public void ExecuteTest(VMUT ut)
        {
            foreach (var test in ut.Tests)
            {
                // Arguments

                var args = new ExecutionEngineArgs()
                {
                    Trigger = (ETriggerType)test.Trigger,
                };

                // Create engine

                using (var engine = new NeoVM.NeoVMFactory().Create(args))
                {
                    engine.LoadScript(test.Script);

                    // Execute Steps

                    foreach (var step in test.Steps)
                    {
                        // Actions

                        foreach (var run in step.Actions)
                        {
                            switch (run)
                            {
                                case VMUTActionType.Execute: engine.Execute(); break;
                                case VMUTActionType.StepInto: engine.StepInto(); break;
                                case VMUTActionType.StepOut: engine.StepOut(); break;
                                case VMUTActionType.StepOver: engine.StepOver(); break;
                            }
                        }

                        // Review results

                        AssertResult(engine, step.State, $"{ut.Category}-{ut.Name}: ");
                    }
                }
            }
        }

        /// <summary>
        /// Assert result
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="result">Result</param>
        /// <param name="message">Message</param>
        private void AssertResult(ExecutionEngineBase engine, VMUTExecutionEngineState result, string message)
        {
            AssertAreEqual(engine.ConsumedGas, result.ConsumedGas, message + "Consumed gas is different");
            AssertAreEqual(engine.State.ToString(), result.State.ToString(), message + "State is different");

            AssertResult(engine.ResultStack, result.ResultStack, message);
            AssertResult(engine.InvocationStack, result.InvocationStack, message);
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

                AssertAreEqual(context.ScriptHash, result[x].ScriptHash, message + "Script hash is different");
                AssertAreEqual((byte)context.NextInstruction, result[x].NextInstruction, message + "Next instruction is different");
                AssertAreEqual(context.InstructionPointer, result[x].InstructionPointer, message + "Instruction pointer is different");

                AssertResult(context.EvaluationStack, result[x].EvaluationStack, message);
                AssertResult(context.AltStack, result[x].AltStack, message);
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
                var item = stack.Peek(x);

                AssertAreEqual(item.Type.ToString(), result[x].Type.ToString(), message + "Stack item type is different");
                AssertAreEqual(item.ToObject().ToString(), result[x].Value, message + "Stack item value is different");
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
            if (a is IEnumerable ca && b is IEnumerable cb)
            {
                a = string.Join(",", ca);
                b = string.Join(",", cb);
            }

            Assert.AreEqual(a, b, message + $" [Expected: {a.ToString()} - Actual: {b.ToString()}]");
        }
    }
}