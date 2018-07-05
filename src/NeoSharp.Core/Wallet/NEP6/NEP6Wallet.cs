using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeoSharp.Core.Wallet.NEP6
{
    public class NEP6Wallet : IWallet
    {
        
        public String Name { get; set; }


        public String Version { get; set; }


        public ScryptParameters Scrypt { get; set; }


        [JsonConverter(typeof(NEP6AccountConverter))]
        public IEnumerable<IWalletAccount> Accounts { get; set; }


        public JObject Extra { get; set; }

        public NEP6Wallet()
        {
            Scrypt = ScryptParameters.Default;
            Accounts = new List<IWalletAccount>();
        }
    }
}
