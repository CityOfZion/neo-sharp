using System;
using NeoSharp.Application.Attributes;
using NeoSharp.Application.Client;
using NeoSharp.Core.Models;
using NeoSharp.Types;

namespace NeoSharp.Application.Controllers
{
    public class PromptInvokesController : IPromptController
    {
        #region Private fields

        private readonly IConsoleHandler _consoleHandler;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="consoleHandler">Console handler</param>
        public PromptInvokesController(IConsoleHandler consoleHandler)
        {
            _consoleHandler = consoleHandler;
        }

        // testinvoke {contract hash} {params} (--attach-neo={amount}, --attach-gas={amount}) (--from-addr={addr})

        /// <summary>
        /// Invoke contract
        /// </summary>
        /// <param name="contractHash">Contract</param>
        /// <param name="body">Body</param>
        [PromptCommand("invoke", Help = "Invoke a contract", Category = "Invokes")]
        public void Invoke(UInt160 contractHash, [PromptCommandParameterBody] object[] args)
        {
            Contract contract = Contract.GetContract(contractHash);
            if (contract == null) throw (new ArgumentNullException("Contract not found"));

            var tx = contract.CreateInvokeTransaction(args);
        }
    }
}