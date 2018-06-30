using System;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;
using NeoSharp.Core.Types.Json;
using NeoSharp.Core.Extensions;
using Newtonsoft.Json;

namespace NeoSharp.Core.Wallet.NEP6
{
    public class NEP6Account : IWalletAccount
    {
        ICrypto _crypto;
        Contract _contract;
        UInt160 _scriptHash;

        public UInt160 ScriptHash
        {
            get { return _scriptHash; }
            set
            {
                _scriptHash = value;
                Address = _scriptHash.ToAddress(_crypto);
            }
        }

        public String Address { get; private set; }
        public String Label { get; set; }
        public bool IsDefault { get; set; }
        public bool Lock { get; set; }
        public String Key { get; set; }
        public Contract Contract
        {
            get { return _contract; }
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
