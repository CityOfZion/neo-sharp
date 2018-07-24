using System;
using System.Collections.Generic;
using NeoSharp.Core.Cryptography;
using Newtonsoft.Json;

namespace NeoSharp.Core.Wallet.NEP6
{
    public class NEP6Wallet : IWallet
    {        
        public string Name { get; set; }

        public string Version { get; set; }

        public ScryptParameters Scrypt { get; set; }

        [JsonConverter(typeof(NEP6AccountConverter))]
        public HashSet<IWalletAccount> Accounts { get; set; }

        //TODO: Replace JObject
        public Object Extra { get; set; }

        public NEP6Wallet()
        {
            Scrypt = ScryptParameters.Default;
            Accounts = new HashSet<IWalletAccount>();
        }
    }
}