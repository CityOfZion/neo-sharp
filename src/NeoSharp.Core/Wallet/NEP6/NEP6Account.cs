using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;
using NeoSharp.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeoSharp.Core.Wallet.NEP6
{
    public class NEP6Account : IWalletAccount
    {
        private readonly ICrypto _crypto;
        private Contract _contract;
        private UInt160 _scriptHash;

        public UInt160 ScriptHash
        {
            get => _scriptHash;
            set
            {
                _scriptHash = value;
                Address = _scriptHash.ToAddress(_crypto);
            }
        }

        public string Address { get; private set; }
        public string Label { get; set; }
        public bool IsDefault { get; set; }
        public bool Lock { get; set; }
        public string Key { get; set; }
        public Contract Contract
        {
            get => _contract;
            set
            {
                _contract = value;
                ScriptHash = _contract.ScriptHash;
            }
        }

        [JsonProperty("extra")]
        public JObject Extra { get; set; }

        public NEP6Account(ICrypto crypto)
        {
            _crypto = crypto;
        }
    }
}
