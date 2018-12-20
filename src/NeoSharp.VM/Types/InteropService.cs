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
        /// Gas ratio
        /// </summary>
        public const uint GasRatio = 100000;

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
            Register("System.Runtime.Platform", GetPlatform);
            Register("System.Runtime.GetTrigger", "Neo.Runtime.GetTrigger", GetTrigger);
            Register("System.Runtime.Log", "Neo.Runtime.Log", "AntShares.Runtime.Log", Log);
            Register("System.Runtime.Notify", "Neo.Runtime.Notify", "AntShares.Runtime.Notify", Notify);
            Register("System.Runtime.Serialize", "Neo.Runtime.Serialize", Serialize);
            Register("System.Runtime.Deserialize", "Neo.Runtime.Deserialize", Deserialize);

            Register("System.ExecutionEngine.GetScriptContainer", GetScriptContainer);
            Register("System.ExecutionEngine.GetExecutingScriptHash", GetExecutingScriptHash);
            Register("System.ExecutionEngine.GetCallingScriptHash", GetCallingScriptHash);
            Register("System.ExecutionEngine.GetEntryScriptHash", GetEntryScriptHash);

            Register("Neo.Enumerator.Create", GetEnumerator);
            Register("Neo.Enumerator.Next", MoveNextEnumerator);
            Register("Neo.Enumerator.Value", GetEnumeratorValue);
            Register("Neo.Enumerator.Concat", ConcatEnumerators);
            Register("Neo.Iterator.Create", GetMapEnumerator);
            Register("Neo.Iterator.Key", GetMapEnumeratorKey);
            Register("Neo.Iterator.Keys", GetKeyEnumerator);
            Register("Neo.Iterator.Values", GetValueEnumerator);

            Register("Neo.Iterator.Next", MoveNextEnumerator);
            Register("Neo.Iterator.Value", GetEnumeratorValue);
        }

        /// <summary>
        /// Register method
        /// </summary>
        /// <param name="name">Method name</param>
        /// <param name="handler">Method delegate</param>
        /// <param name="gas">Gas</param>
        public void Register(string name, Func<IExecutionEngine, bool> handler, uint gas = 1)
        {
            Register(name, null, null, handler, gas);
        }

        /// <summary>
        /// Register method
        /// </summary>
        /// <param name="name">Method name</param>
        /// <param name="alias">Alias</param>
        /// <param name="handler">Method delegate</param>
        /// <param name="gas">Gas</param>
        public void Register(string name, string alias, Func<IExecutionEngine, bool> handler, uint gas = 1)
        {
            Register(name, alias, null, handler, gas);
        }

        public void Register(string name, string alias1, string alias2, Func<IExecutionEngine, bool> handler, uint gas = 1)
        {
            var entry = new InteropServiceEntry(name, GetMethodHash(name), handler, checked(gas * GasRatio));
            _entries[entry.MethodHash] = entry;

            if (string.IsNullOrEmpty(alias1)) return;

            entry = new InteropServiceEntry(alias1, GetMethodHash(alias1), handler, checked(gas * GasRatio));
            _entries[entry.MethodHash] = entry;

            if (string.IsNullOrEmpty(alias2)) return;

            entry = new InteropServiceEntry(alias2, GetMethodHash(alias2), handler, checked(gas * GasRatio));
            _entries[entry.MethodHash] = entry;

        }

        /// <summary>
        /// Invoke method
        /// </summary>
        /// <param name="method">Method</param>
        /// <param name="engine">Execution engine</param>
        /// <returns>Return false if something is wrong</returns>
        public bool Invoke(byte[] method, IExecutionEngine engine)
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

        private static bool GetPlatform(IExecutionEngine engine)
        {
            engine.CurrentContext.EvaluationStack.Push(Encoding.ASCII.GetBytes("NEO"));
            return true;
        }

        private static bool GetTrigger(IExecutionEngine engine)
        {
            engine.CurrentContext.EvaluationStack.Push((int)engine.Trigger);

            return true;
        }

        private bool Log(IExecutionEngine engine)
        {
            var message = Encoding.UTF8.GetString(engine.CurrentContext.EvaluationStack.PopByteArray());

            OnLog?.Invoke(this, new LogEventArgs(engine.CurrentContext.ScriptHash, message));

            return true;
        }

        private bool Notify(IExecutionEngine engine)
        {
            if (!engine.CurrentContext.EvaluationStack.TryPop(out var stackItem))
            {
                return false;
            }

            using (stackItem)
            {
                OnNotify?.Invoke(this, new NotifyEventArgs(engine.CurrentContext.ScriptHash, stackItem));
            }

            return true;
        }

        private static bool Serialize(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;

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

        private static bool Deserialize(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;
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


        private static bool GetScriptContainer(IExecutionEngine engine)
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

        private static bool GetExecutingScriptHash(IExecutionEngine engine)
        {
            var ctx = engine.CurrentContext;
            if (ctx == null) return false;

            ctx.EvaluationStack.Push(ctx.ScriptHash);

            return true;
        }

        private static bool GetCallingScriptHash(IExecutionEngine engine)
        {
            var ctx = engine.CallingContext;
            if (ctx == null) return false;

            ctx.EvaluationStack.Push(ctx.ScriptHash);

            return true;
        }

        private static bool GetEntryScriptHash(IExecutionEngine engine)
        {
            var ctx = engine.EntryContext;
            if (ctx == null) return false;

            ctx.EvaluationStack.Push(ctx.ScriptHash);

            return true;
        }

        private static bool GetEnumerator(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;
            if (!stack.TryPop(out var stackItem)) return false;

            using (stackItem)
            {
                if (!(stackItem is ArrayStackItemBase array)) return false;

                stack.Push(stack.CreateInterop(array.GetEnumerator()));
            }

            return true;
        }

        private static bool MoveNextEnumerator(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;
            var enumerator = stack.PopObject<IEnumerator<StackItemBase>>();
            if (enumerator == null) return false;

            stack.Push(enumerator.MoveNext());

            return true;
        }

        private static bool GetEnumeratorValue(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;
            var enumerator = stack.PopObject<IEnumerator<StackItemBase>>();
            if (enumerator == null) return false;

            stack.Push(enumerator.Current);

            return true;
        }

        private static bool ConcatEnumerators(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;
            var enumerator1 = stack.PopObject<IEnumerator<StackItemBase>>();
            if (enumerator1 == null) return false;
            var enumerator2 = stack.PopObject<IEnumerator<StackItemBase>>();
            if (enumerator2 == null) return false;

            var concatenatedEnumerator = new ConcatenatedEnumerator(enumerator1, enumerator2);

            stack.Push(stack.CreateInterop(concatenatedEnumerator));

            return true;
        }

        private static bool GetMapEnumerator(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;
            if (!stack.TryPop(out var stackItem)) return false;

            using (stackItem)
            {
                if (!(stackItem is MapStackItemBase map)) return false;

                stack.Push(stack.CreateInterop(new KeyEnumerator(map.GetEnumerator())));
            }

            return true;
        }

        private static bool GetMapEnumeratorKey(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;
            var keyEnumerator = stack.PopObject<KeyEnumerator>();
            if (keyEnumerator == null) return false;

            stack.Push(keyEnumerator.CurrentKey);

            return false;
        }

        private static bool GetKeyEnumerator(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;
            var keyEnumerator = stack.PopObject<KeyEnumerator>();
            if (keyEnumerator == null) return false;

            var projectingEnumerator = new ProjectingEnumerator<KeyEnumerator>(keyEnumerator, ke => ke.CurrentKey);

            stack.Push(stack.CreateInterop(projectingEnumerator));

            return true;
        }

        private static bool GetValueEnumerator(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;
            var keyEnumerator = stack.PopObject<KeyEnumerator>();
            if (keyEnumerator == null) return false;

            var projectingEnumerator = new ProjectingEnumerator<KeyEnumerator>(keyEnumerator, ke => ke.Current);

            stack.Push(stack.CreateInterop(projectingEnumerator));

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
