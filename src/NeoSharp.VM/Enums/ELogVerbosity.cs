using System;

namespace NeoSharp.VM
{
    [Flags]
    public enum ELogVerbosity : byte
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Enable step into logs
        /// </summary>
        StepInto = 0x01,

        /// <summary>
        /// ExecutionContextStack changes
        /// </summary>
        ExecutionContextStackChanges = 0x02,
        /// <summary>
        /// EvaluationStack changes
        /// </summary>
        EvaluationStackChanges = 0x04,
        /// <summary>
        /// AltStack changes
        /// </summary>
        AltStackChanges = 0x08,
        /// <summary>
        /// ResultStack changes
        /// </summary>
        ResultStackChanges = 0x10,

        /// <summary>
        /// All
        /// </summary>
        All = StepInto | ExecutionContextStackChanges | EvaluationStackChanges | AltStackChanges| ResultStackChanges,
        /// <summary>
        /// Stack changes
        /// </summary>
        StackChanges = ExecutionContextStackChanges | EvaluationStackChanges | AltStackChanges | ResultStackChanges,
    }
}