using NeoSharp.Core.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        [JsonProperty("extra")]
        public JObject Extra { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public NEP6Wallet()
        {
            Scrypt = ScryptParameters.Default;
            Accounts = new IWalletAccount[0];
        }
    }
}