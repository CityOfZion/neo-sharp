namespace NeoSharp.VM.NeoVM
{
    public class NeoVMFactory : IVMFactory
    {
        public ExecutionEngineBase Create(ExecutionEngineArgs args)
        {
            return new ApplicationEngine(args);
        }
    }
}