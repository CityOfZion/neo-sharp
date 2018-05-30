namespace NeoSharp.VM
{
    public interface IScriptTable
    {
        /// <summary>
        /// Get script of this hash
        /// </summary>
        /// <param name="scriptHash">Script hash</param>
        /// <param name="isDynamicInvoke">Is dynamic invoke</param>
        /// <returns>Script or NULL</returns>
        byte[] GetScript(byte[] scriptHash, bool isDynamicInvoke);
    }
}