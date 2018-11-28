namespace NeoSharp.VM.NeoVM
{
    public class ContractScriptTable : IContractScriptTable
    {
        private readonly IScriptTable _scriptTable;

        public ContractScriptTable(IScriptTable scriptTable)
        {
            _scriptTable = scriptTable;
        }

        public byte[] GetScript(byte[] scriptHash)
        {
            return _scriptTable.GetScript(scriptHash, false);
        }

        public byte[] GetScript(byte[] scriptHash, bool isDynamicInvoke)
        {
            return _scriptTable.GetScript(scriptHash, isDynamicInvoke);
        }
    }
}