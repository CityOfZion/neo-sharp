using System;
using NeoSharp.Application.Attributes;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Application.Client
{
    public partial class Prompt : IPrompt
    {
        // testinvoke {contract hash} {params} (--attach-neo={amount}, --attach-gas={amount}) (--from-addr={addr})
        
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
    }
}