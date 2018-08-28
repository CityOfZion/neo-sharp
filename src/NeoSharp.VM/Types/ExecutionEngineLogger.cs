namespace NeoSharp.VM
{
    public class ExecutionEngineLogger
    {
        /// <summary>
        /// Delegate for receive operations
        /// </summary>
        /// <param name="context">Execution context</param>
        public delegate void delOnStepInto(IExecutionContext context);

        #region Properties

        /// <summary>
        /// Verbosity
        /// </summary>
        public ELogVerbosity Verbosity { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// On StepInto
        /// </summary>
        public event delOnStepInto OnStepInto;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="verbosity">Verbosity</param>
        public ExecutionEngineLogger(ELogVerbosity verbosity)
        {
            Verbosity = verbosity;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raise OnStepInto
        /// </summary>
        /// <param name="context">Context</param>
        public virtual void RaiseOnStepInto(IExecutionContext context)
        {
            OnStepInto?.Invoke(context);
        }

        #endregion
    }
}