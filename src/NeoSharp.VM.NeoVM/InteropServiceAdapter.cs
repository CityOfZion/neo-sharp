namespace NeoSharp.VM.NeoVM
{
    public class InteropServiceAdapter : Neo.VM.IInteropService
    {
        /// <summary>
        /// Base service
        /// </summary>
        private readonly InteropService _service;

        /// <summary>
        /// This class is dependent of the ApplicationEngine, 
        ///  because is not possible to convert from ExecutionEngine to IExecutionEngine right now
        /// </summary>
        private readonly ExecutionEngineBase _engine;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Execution Engine</param>
        /// <param name="service">Base Service</param>
        public InteropServiceAdapter(ExecutionEngineBase engine, InteropService service)
        {
            _service = service;
            _engine = engine;
        }

        /// <summary>
        /// Invoke of NeoVM
        /// </summary>
        /// <param name="method">Method</param>
        /// <param name="engine">Engine</param>
        /// <returns>Return true if works</returns>
        public bool Invoke(byte[] method, Neo.VM.ExecutionEngine engine)
        {
            return _service.Invoke(method, _engine);
        }
    }
}