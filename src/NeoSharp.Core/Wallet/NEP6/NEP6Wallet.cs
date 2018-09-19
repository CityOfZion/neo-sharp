using System;
using System.Collections.Generic;
using NeoSharp.Core.Cryptography;
using Newtonsoft.Json;

namespace NeoSharp.Core.Wallet.NEP6
{
    public class NEP6Wallet : IWallet
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("scrypt")]
        public ScryptParameters Scrypt { get; set; }

        [JsonProperty("accounts")]
        [JsonConverter(typeof(NEP6AccountConverter))]
        public IWalletAccount[] Accounts { get; set; }

        //TODO #359: Replace JObject
        [JsonProperty("extra")]
        public Object Extra { get; set; }

        public NEP6Wallet()
        {
            Scrypt = ScryptParameters.Default;
            Accounts = new IWalletAccount[0];
        }
    }
}