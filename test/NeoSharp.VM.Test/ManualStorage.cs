using System.Collections.Generic;
using System.Linq;
using NeoSharp.Core.Models;
using NeoSharp.Core.VM;

namespace NeoSharp.VM.Test
{
    public class ManualStorage : DataCache<StorageKey, StorageValue>
    {
        private readonly IDictionary<StorageKey, StorageValue> _storage = new Dictionary<StorageKey, StorageValue>();

        public override void DeleteInternal(StorageKey key) => _storage.Remove(key);

        protected override void AddInternal(StorageKey key, StorageValue value) => _storage.Add(key, value);

        protected override void UpdateInternal(StorageKey key, StorageValue value) => _storage[key] = value;

        protected override IEnumerable<KeyValuePair<StorageKey, StorageValue>> FindInternal(byte[] key_prefix)
        {
            foreach (var keyValue in _storage)
            {
                var key = keyValue.Key.ScriptHash.ToArray().Concat(keyValue.Key.Key).ToArray();

                if (!key_prefix.SequenceEqual(key.Take(key.Length))) continue;

                yield return keyValue;
            }
        }

        protected override StorageValue TryGetInternal(StorageKey key)
        {
            if (!_storage.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }
    }
}