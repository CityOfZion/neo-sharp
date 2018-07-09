using NeoSharp.Application.Attributes;
using NeoSharp.Core.Wallet;
using System.IO;
using System.Security;

namespace NeoSharp.Application.Client
{
    public partial class Prompt : IPrompt
    {
        [PromptCommand("wallet create", Category = "Wallet", Help = "Create a new wallet")]
        private void WalletCreateCommand(string fileName)
        {
            try
            {
                _walletManager.CreateWallet(fileName);
                SecureString secureString = _consoleReader.ReadPassword();
                _walletManager.CreateAccount(secureString);
            }
            catch(System.Exception exception)
            {
                _consoleWriter.WriteLine(exception.Message, ConsoleOutputStyle.Error);
            }

        }

        [PromptCommand("wallet open", Category = "Wallet", Help = "Open wallet")]
        private void WalletOpenCommand(string fileName)
        {
            try
            {
                _walletManager.Load(fileName);
                SecureString secureString = _consoleReader.ReadPassword();
                _walletManager.UnlockAllAccounts(secureString);
            }catch (System.Exception exception)
            {
                _consoleWriter.WriteLine(exception.Message, ConsoleOutputStyle.Error);
            }
        }

        [PromptCommand("wallet close", Category = "Wallet", Help = "Close wallet")]
        private void WalletCloseCommand()
        {
            try{
                _walletManager.Close();
            }catch (System.Exception exception)
            {
                _consoleWriter.WriteLine(exception.Message, ConsoleOutputStyle.Error);
            }
        }

        [PromptCommand("import wif", Category = "Wallet", Help = "Close wallet")]
        private void ImportWif(string wif)
        {
            try
            {
                SecureString secureString = _consoleReader.ReadPassword();
                _walletManager.ImportWif(wif, secureString);
            }catch (System.Exception exception)
            {
                _consoleWriter.WriteLine(exception.Message, ConsoleOutputStyle.Error);
            }
        }

        [PromptCommand("import nep2", Category = "Wallet", Help = "Close wallet")]
        private void ImportNep2(string nep2key)
        {
            try
            {
                SecureString secureString = _consoleReader.ReadPassword();
                _walletManager.ImportEncryptedWif(nep2key, secureString);
            }catch (System.Exception exception)
            {
                _consoleWriter.WriteLine(exception.Message, ConsoleOutputStyle.Error);
            }
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