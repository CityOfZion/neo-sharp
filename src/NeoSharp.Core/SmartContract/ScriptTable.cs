using NeoSharp.Core.Persistence;
using NeoSharp.Types;
using NeoSharp.VM;

namespace NeoSharp.Core.SmartContract
{
    public class ScriptTable : IScriptTable
    {
        private readonly IRepository _repository;

        public ScriptTable(IRepository repository)
        {
            _repository = repository;
        }

        public byte[] GetScript(byte[] scriptHash, bool isDynamicInvoke)
        {
            var contractHash = new UInt160(scriptHash);
            var contractTask = _repository.GetContract(contractHash);

            if (contractTask == null) return null;

            contractTask.Wait();

            return contractTask.Result?.Script;
        }
    }
}