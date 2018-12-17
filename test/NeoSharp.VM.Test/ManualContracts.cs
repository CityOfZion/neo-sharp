using System.Collections.Generic;
using System.Linq;
using NeoSharp.Core.Models;
using NeoSharp.Core.VM;
using NeoSharp.Types;

namespace NeoSharp.VM.Test
{
    public class ManualContracts : DataCache<UInt160, Contract>, IScriptTable
    {
        private readonly IDictionary<UInt160, Contract> _entries = new Dictionary<UInt160, Contract>();

        public override void DeleteInternal(UInt160 key) => _entries.Remove(key);

        protected override void UpdateInternal(UInt160 key, Contract value) => _entries[key] = value;

        protected override void AddInternal(UInt160 key, Contract value) => _entries[key] = value;

        public byte[] GetScript(byte[] scriptHash, bool isDynamicInvoke)
        {
            if (_entries.TryGetValue(new UInt160(scriptHash), out var contract))
            {
                if (isDynamicInvoke && !contract.HasDynamicInvoke) return null;

                return contract.Script;
            }

            return null;
        }

        protected override IEnumerable<KeyValuePair<UInt160, Contract>> FindInternal(byte[] key_prefix)
        {
            foreach (var keyValue in _entries)
            {
                if (!keyValue.Key.ToArray().Take(key_prefix.Length).SequenceEqual(key_prefix)) continue;

                yield return keyValue;
            }
        }

        protected override Contract TryGetInternal(UInt160 key)
        {
            if (!_entries.TryGetValue(key, out var value)) return null;

            return value;
        }
    }
}