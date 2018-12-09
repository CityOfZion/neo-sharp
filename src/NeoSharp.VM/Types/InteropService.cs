using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NeoSharp.VM
{
    public class InteropService
    {
        /// <summary>
        /// Set Max Item Size
        /// </summary>
        public const uint MaxItemSize = 1024 * 1024;

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
        /// Method hashes cache
        /// </summary>
        private static readonly Dictionary<string, uint> MethodHashes = new Dictionary<string, uint>();

        /// <summary>
        /// Cache dictionary
        /// </summary>
        private readonly IDictionary<uint, InteropServiceEntry> _entries = new SortedDictionary<uint, InteropServiceEntry>();

        /// <summary>
        /// Stack serializer
        /// </summary>
        private static readonly StackItemBinarySerializer Serializer = new StackItemBinarySerializer();

        /// <summary>
        /// Get method
        /// </summary>
        /// <param name="methodName">Method name</param>
        public InteropServiceEntry this[string methodName] => _entries.TryGetValue(GetMethodHash(methodName), out var entry) ? entry : null;

        /// <summary>
        /// Get method
        /// </summary>
        /// <param name="methodHash">Method hash</param>
        public InteropServiceEntry this[uint methodHash] => _entries.TryGetValue(methodHash, out var entry) ? entry : null;

        /// <summary>
        /// Constructor
        /// </summary>
        public InteropService()
        {
            Register("Neo.Runtime.GetTrigger", NeoRuntimeGetTrigger, 1);
            Register("Neo.Runtime.Log", NeoRuntimeLog, 1);
            Register("Neo.Runtime.Notify", NeoRuntimeNotify, 1);
            Register("Neo.Runtime.Serialize", "System.Runtime.Serialize", RuntimeSerialize, 1);
            Register("Neo.Runtime.Deserialize", "System.Runtime.Deserialize", RuntimeDeserialize, 1);

            Register("System.ExecutionEngine.GetScriptContainer", GetScriptContainer, 1);
            Register("System.ExecutionEngine.GetExecutingScriptHash", GetExecutingScriptHash, 1);
            Register("System.ExecutionEngine.GetCallingScriptHash", GetCallingScriptHash, 1);
            Register("System.ExecutionEngine.GetEntryScriptHash", GetEntryScriptHash, 1);
        }

        /// <summary>
        /// Register method
        /// </summary>
        /// <param name="name">Method name</param>
        /// <param name="handler">Method delegate</param>
        /// <param name="gas">Gas</param>
        public void Register(string name, Func<ExecutionEngineBase, bool> handler, uint gas = 1)
        {
            Register(name, null, handler, gas);
        }

        /// <summary>
        /// Register method
        /// </summary>
        /// <param name="name">Method name</param>
        /// <param name="alias">Alias</param>
        /// <param name="handler">Method delegate</param>
        /// <param name="gas">Gas</param>
        public void Register(string name, string alias, Func<ExecutionEngineBase, bool> handler, uint gas = 1)
        {
            var entry = new InteropServiceEntry(name, GetMethodHash(name), handler, gas);
            _entries[entry.MethodHash] = entry;

            if (string.IsNullOrEmpty(alias)) return;

            entry = new InteropServiceEntry(alias, GetMethodHash(alias), handler, gas);
            _entries[entry.MethodHash] = entry;
        }

        /// <summary>
        /// Invoke method
        /// </summary>
        /// <param name="method">Method</param>
        /// <param name="engine">Execution engine</param>
        /// <returns>Return false if something is wrong</returns>
        public bool Invoke(byte[] method, ExecutionEngineBase engine)
        {
            var hash = method.Length == 4
                ? BitConverter.ToUInt32(method, 0)
                : GetMethodHash(Encoding.ASCII.GetString(method));

            void InvokeOnSysCall(SysCallResult result) => OnSysCall?.Invoke(this, new SysCallArgs(engine, GetMethodName(hash), hash, result));

            if (!_entries.TryGetValue(hash, out var entry))
            {
                InvokeOnSysCall(SysCallResult.NotFound);

                return false;
            }

            if (!engine.IncreaseGas(entry.GasCost))
            {
                InvokeOnSysCall(SysCallResult.OutOfGas);

                return false;
            }

            var ret = entry.MethodHandler(engine);

            InvokeOnSysCall(ret ? SysCallResult.True : SysCallResult.False);

            return ret;
        }

        #region Delegates

        private static bool RuntimeSerialize(ExecutionEngineBase engine)
        {
            var ctx = engine.CurrentContext;
            if (ctx == null) return false;

            var stack = ctx.EvaluationStack;

            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                try
                {
                    using (var stackItem = stack.Pop())
                    {
                        Serializer.Serialize(stackItem, writer);
                    }
                }
                catch
                {
                    return false;
                }

                writer.Flush();

                if (ms.Length > MaxItemSize)
                {
                    return false;
                }

                stack.Push(ms.ToArray());
            }

            return true;
        }

        private static bool RuntimeDeserialize(ExecutionEngineBase engine)
        {
            var ctx = engine.CurrentContext;
            if (ctx == null) return false;

            var stack = ctx.EvaluationStack;
            byte[] data;

            using (var stackItem = stack.Pop())
            {
                data = stackItem.ToByteArray();

                if (data == null) return false;
            }

            using (var ms = new MemoryStream(data, false))
            using (var reader = new BinaryReader(ms))
            {
                StackItemBase item;

                try
                {
                    item = Serializer.Deserialize(stack, reader);
                }
                catch
                {
                    return false;
                }

                stack.Push(item);
            }

            return true;
        }

        private static bool NeoRuntimeGetTrigger(ExecutionEngineBase engine)
        {
            var ctx = engine.CurrentContext;
            if (ctx == null) return false;

            ctx.EvaluationStack.Push((int)engine.Trigger);

            return true;
        }

        public void RaiseOnLog(LogEventArgs e) => OnLog?.Invoke(this, e);

        public void RaiseOnNotify(NotifyEventArgs e) => OnNotify?.Invoke(this, e);

        private bool NeoRuntimeLog(ExecutionEngineBase engine)
        {
            var ctx = engine.CurrentContext;
            if (ctx == null) return false;

            if (!ctx.EvaluationStack.TryPop(out StackItemBase it))
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
                RaiseOnLog(new LogEventArgs(ctx.ScriptHash, message ?? ""));
            }

            return true;
        }

        private bool NeoRuntimeNotify(ExecutionEngineBase engine)
        {
            var ctx = engine.CurrentContext;
            if (ctx == null) return false;

            if (!ctx.EvaluationStack.TryPop(out StackItemBase it))
            {
                return false;
            }

            using (it)
            {
                RaiseOnNotify(new NotifyEventArgs(ctx.ScriptHash, it));
            }

            return true;
        }

        private static bool GetScriptContainer(ExecutionEngineBase engine)
        {
            if (engine.MessageProvider == null)
            {
                return false;
            }

            var ctx = engine.CurrentContext;
            if (ctx == null) return false;

            ctx.EvaluationStack.PushObject(engine.MessageProvider);

            return true;
        }

        private static bool GetExecutingScriptHash(ExecutionEngineBase engine)
        {
            var ctx = engine.CurrentContext;
            if (ctx == null) return false;

            ctx.EvaluationStack.Push(ctx.ScriptHash);

            return true;
        }

        private static bool GetCallingScriptHash(ExecutionEngineBase engine)
        {
            var ctx = engine.CallingContext;
            if (ctx == null) return false;

            ctx.EvaluationStack.Push(ctx.ScriptHash);

            return true;
        }

        private static bool GetEntryScriptHash(ExecutionEngineBase engine)
        {
            var ctx = engine.EntryContext;
            if (ctx == null) return false;

            ctx.EvaluationStack.Push(ctx.ScriptHash);

            return true;
        }

        #endregion

        /// <summary>
        /// Convert method to hash
        /// </summary>
        /// <param name="methodName">Method</param>
        /// <returns>Hash</returns>
        private static uint GetMethodHash(string methodName)
        {
            if (MethodHashes.TryGetValue(methodName, out var methodHash))
                return methodHash;

            using (var sha = SHA256.Create())
            {
                methodHash = BitConverter.ToUInt32(sha.ComputeHash(Encoding.ASCII.GetBytes(methodName)), 0);
            }

            MethodHashes[methodName] = methodHash;

            return methodHash;
        }

        /// <summary>
        /// Convert method hash to name
        /// </summary>
        /// <param name="methodHash">Method name</param>
        /// <returns>Method</returns>
        private static string GetMethodName(uint methodHash)
        {
            var ret = MethodHashes
                .Where(u => u.Value == methodHash)
                .Select(u => u.Key)
                .FirstOrDefault();

            return !string.IsNullOrEmpty(ret) ? ret : "Unknown";
        }
    }
}
