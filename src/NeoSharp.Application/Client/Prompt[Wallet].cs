using NeoSharp.Application.Attributes;
using NeoSharp.Core.Wallet;
using System.IO;
using System.Security;

namespace NeoSharp.Application.Client
{
    public partial class Prompt : IPrompt
    {
        [PromptCommand("wallet create", Category = "Wallet", Help = "Create a new wallet")]
        private void WalletCreateCommand(FileInfo file)
        {
            try
            {
                _walletManager.CreateWallet(file);
            }
            catch(System.ArgumentException)
            {
                _consoleWriter.WriteLine($"File '{file.FullName}' already exist, please provide a new one", ConsoleOutputStyle.Error);
                return;
            }

            SecureString secureString = _consoleReader.ReadPassword();
            //TODO: Continue / Send password to save it in json
            _walletManager.CreateAccount();
        }

        [PromptCommand("wallet open", Category = "Wallet", Help = "Open wallet")]
        private void WalletOpenCommand(FileInfo file)
        {
            _walletManager.Load(file);

            SecureString secureString = _consoleReader.ReadPassword();

            _walletManager.UnlockAllAccounts(secureString.ToString());
        }

        [PromptCommand("wallet close", Category = "Wallet", Help = "Close wallet")]
        private void WalletCloseCommand()
        {
            _walletManager.Close();
        }

        [PromptCommand("import wif", Category = "Wallet", Help = "Close wallet")]
        private void ImportWif(string wif)
        {
            _walletManager.Import(wif);
        }

        [PromptCommand("import nep2", Category = "Wallet", Help = "Close wallet")]
        private void ImportNep2(string nep2key)
        {
            SecureString secureString = _consoleReader.ReadPassword();
            _walletManager.Import(nep2key, secureString.ToString());
        }

        /*
        TODO WALLET:
        wallet delete_addr {addr}
        wallet delete_token {token_contract_hash}
        wallet alias {addr} {title}

        import multisig_addr {pubkey in wallet} {minimum # of signatures required} {signing pubkey 1} {signing pubkey 2}...
        import watch_addr {address}
        import token {token_contract_hash}
        export wif {address}
        export nep2 {address}

         */

        /*
        TODO TRANSACTION MANAGER:
        wallet rebuild {start block}
        wallet claim
        wallet tkn_send {token symbol} {address_from} {address to} {amount} 
        wallet tkn_send_from {token symbol} {address_from} {address to} {amount}
        wallet unspent
        wallet tkn_approve {token symbol} {address_from} {address to} {amount}
        wallet tkn_allowance {token symbol} {address_from} {address to}
        wallet tkn_mint {token symbol} {mint_to_addr} (--attach-neo={amount}, --attach-gas={amount})
        wallet tkn_register {addr} ({addr}...) (--from-addr={addr})

        withdraw_request {asset_name} {contract_hash} {to_addr} {amount}
        withdraw holds # lists all current holds
        withdraw completed # lists completed holds eligible for cleanup
        withdraw cancel # cancels current holds
        withdraw cleanup # cleans up completed holds
        withdraw # withdraws the first hold availabe
        withdraw all # withdraw all holds available

        send {assetId or name} {address} {amount} (--from-addr={addr})
        sign {transaction in JSON format}

         */
    }
}