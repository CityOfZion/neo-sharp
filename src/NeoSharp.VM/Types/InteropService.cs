using System;
using System.Collections.Generic;
using NeoSharp.VM.Types;

namespace NeoSharp.VM
{
    public class InteropService
    {
        /// <summary>
        /// Notify event
        /// </summary>
        public event EventHandler<NotifyEventArgs> OnNotify;
        /// <summary>
        /// Log event
        /// </summary>
        public event EventHandler<LogEventArgs> OnLog;
        /// <summary>
        /// Syscall event
        /// </summary>
        public event EventHandler<SysCallArgs> OnSysCall;

        /// <summary>
        /// Cache dictionary
        /// </summary>
        private readonly SortedDictionary<string, InteropServiceEntry> _entries = new SortedDictionary<string, InteropServiceEntry>();

        /// <summary>
        /// Get method
        /// </summary>
        /// <param name="method">Method</param>
        public InteropServiceEntry this[string method]
        {
            get
            {
                if (!_entries.TryGetValue(method, out var func)) return null;
                return func;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InteropService()
        {
            // TODO: GAS COST https://github.com/neo-project/neo/blob/b5926fe88d25c8aab2028c0ff7acad2c1d982bad/neo/SmartContract/ApplicationEngine.cs#L383

            Register("Neo.Runtime.GetTrigger", NeoRuntimeGetTrigger);
            Register("Neo.Runtime.Log", NeoRuntimeLog);
            Register("Neo.Runtime.Notify", NeoRuntimeNotify);
            //Register("Neo.Runtime.Serialize", Runtime_Serialize);
            //Register("Neo.Runtime.Deserialize", Runtime_Deserialize);

            Register("System.ExecutionEngine.GetScriptContainer", GetScriptContainer);
            Register("System.ExecutionEngine.GetExecutingScriptHash", GetExecutingScriptHash);
            Register("System.ExecutionEngine.GetCallingScriptHash", GetCallingScriptHash);
            Register("System.ExecutionEngine.GetEntryScriptHash", GetEntryScriptHash);
        }

        /// <summary>
        /// Register method
        /// </summary>
        /// <param name="method">Method name</param>
        /// <param name="synonymous">Synonymous</param>
        /// <param name="handler">Method delegate</param>
        /// <param name="gas">Gas</param>
        protected void Register(string method, string synonymous, InteropServiceEntry.delHandler handler, uint gas = 1)
        {
            var entry = new InteropServiceEntry(handler, gas);

            _entries[method] = entry;
            _entries[synonymous] = entry;
        }

        /// <summary>
        /// Register method
        /// </summary>
        /// <param name="method">Method name</param>
        /// <param name="handler">Method delegate</param>
        /// <param name="gas">Gas</param>
        protected void Register(string method, InteropServiceEntry.delHandler handler, uint gas = 1)
        {
            _entries[method] = new InteropServiceEntry(handler, gas);
        }

        /// <summary>
        /// Clear entries
        /// </summary>
        public void Clear()
        {
            _entries.Clear();
        }
        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="method">Method name</param>
        /// <param name="engine">Execution engine</param>
        /// <returns>Return false if something wrong</returns>
        public bool Invoke(string method, IExecutionEngine engine)
        {
            if (!_entries.TryGetValue(method, out var entry))
            {
                OnSysCall?.Invoke(this, new SysCallArgs(engine, method, SysCallArgs.EResult.NotFound));

                return false;
            }

            if (!engine.IncreaseGas(entry.GasCost))
            {
                OnSysCall?.Invoke(this, new SysCallArgs(engine, method, SysCallArgs.EResult.OutOfGas));

                return false;
            }

            var ret = entry.Handler(engine);

            OnSysCall?.Invoke(this, new SysCallArgs(engine, method, ret ? SysCallArgs.EResult.True : SysCallArgs.EResult.False));

            return ret;
        }

        #region Delegates

        static bool NeoRuntimeGetTrigger(IExecutionEngine engine)
        {
            using (var ctx = engine.CurrentContext)
            using (var item = engine.CreateInteger((int)engine.Trigger))
                ctx.EvaluationStack.Push(item);

            return true;
        }

        bool NeoRuntimeLog(IExecutionEngine engine)
        {
            using (var ctx = engine.CurrentContext)
            {
                if (ctx == null) return false;

                if (!ctx.EvaluationStack.TryPop(out IStackItem it))
                {
                    return false;
                }

                using (it)
                {
                    if (OnLog == null)
                    {
                        return true;
                    }

                    // Get string

                    var message = it.ToString();
                    OnLog.Invoke(this, new LogEventArgs(engine.MessageProvider, ctx?.ScriptHash, message ?? ""));
                }
            }

            return true;
        }

        bool NeoRuntimeNotify(IExecutionEngine engine)
        {
            using (var ctx = engine.CurrentContext)
            {
                if (ctx == null) return false;

                if (!ctx.EvaluationStack.TryPop(out IStackItem it))
                {
                    return false;
                }

                using (it)
                {
                    OnNotify?.Invoke(this, new NotifyEventArgs(engine.MessageProvider, ctx?.ScriptHash, it));
                }
            }

            return true;
        }

        static bool GetScriptContainer(IExecutionEngine engine)
        {
            if (engine.MessageProvider == null)
            {
                return false;
            }

            using (var current = engine.CurrentContext)
            using (var item = engine.CreateInterop(engine.MessageProvider))
                current.EvaluationStack.Push(item);

            return true;
        }

        static bool GetExecutingScriptHash(IExecutionEngine engine)
        {
            using (var ctx = engine.CurrentContext)
            {
                if (ctx == null) return false;

                using (var item = engine.CreateByteArray(ctx.ScriptHash))
                    ctx.EvaluationStack.Push(item);
            }

            return true;
        }

        static bool GetCallingScriptHash(IExecutionEngine engine)
        {
            using (var ctx = engine.CallingContext)
            {
                if (ctx == null) return false;

                using (var current = engine.CurrentContext)
                using (var item = engine.CreateByteArray(ctx.ScriptHash))
                    current.EvaluationStack.Push(item);
            }

            return true;
        }

        static bool GetEntryScriptHash(IExecutionEngine engine)
        {
            using (var ctx = engine.EntryContext)
            {
                if (ctx == null) return false;

                using (var current = engine.CurrentContext)
                using (var item = engine.CreateByteArray(ctx.ScriptHash))
                    current.EvaluationStack.Push(item);
            }

            return true;
        }

        #endregion
    }
}
