using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.SmartContract.ContractParameters;
using NeoSharp.Core.Wallet.Invocation;
using NeoSharp.TestHelpers;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.SmartContracts
{
    [TestClass]
    public class UtInvocationProcess : TestBase
    {
        [TestMethod]
        public void TestInvoke()
        {
            var scriptHash = new UInt160();

            var nullExecutionEngine = new NullExecutionEngine();

            var stackItemStackMock = new NullStackItemsStack(nullExecutionEngine);

            nullExecutionEngine.PublicStackItemsStack = stackItemStackMock;

            var invocationProcess = new InvocationProcess(nullExecutionEngine);
            var intParameter = new IntegerContractParameter(1);
            var contractParameters = new ContractParameter[] { intParameter };

            var invocationResult = invocationProcess.Invoke(scriptHash, contractParameters);
            Assert.IsNotNull(invocationResult);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestInvokeScriptNull()
        {
            var nullExecutionEngine = new NullExecutionEngine();

            var stackItemStackMock = new NullStackItemsStack(nullExecutionEngine);

            nullExecutionEngine.PublicStackItemsStack = stackItemStackMock;

            var invocationProcess = new InvocationProcess(nullExecutionEngine);
            var intParameter = new IntegerContractParameter(1);
            var contractParameters = new ContractParameter[] { intParameter };

            var invocationResult = invocationProcess.Invoke(null, contractParameters);
            Assert.IsNotNull(invocationResult);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestInvokeContractNull()
        {
            var scriptHash = new UInt160();

            var nullExecutionEngine = new NullExecutionEngine();

            var stackItemStackMock = new NullStackItemsStack(nullExecutionEngine);

            nullExecutionEngine.PublicStackItemsStack = stackItemStackMock;

            var invocationProcess = new InvocationProcess(nullExecutionEngine);

            var invocationResult = invocationProcess.Invoke(scriptHash, null);
            Assert.IsNotNull(invocationResult);
        }
    }
}
