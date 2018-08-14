using NeoSharp.Application.Attributes;
using NeoSharp.Application.Client;
using NeoSharp.Core.Types;
using NeoSharp.Core.Wallet;

namespace NeoSharp.Application.Controllers
{
    public class PromptWalletController : IPromptController
    {
        #region Private fields

        /// <summary>
        /// The wallet.
        /// </summary>
        private readonly IWalletManager _walletManager;
        private readonly IConsoleWriter _consoleWriter;
        private readonly IConsoleReader _consoleReader;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="walletManager">Wallet manager</param>
        /// <param name="consoleWriter">Console writter</param>
        /// <param name="consoleReader">Console reader</param>
        public PromptWalletController(IWalletManager walletManager, IConsoleWriter consoleWriter, IConsoleReader consoleReader)
        {
            _walletManager = walletManager;
            _consoleReader = consoleReader;
            _consoleWriter = consoleWriter;
        }

        [PromptCommand("wallet create", Category = "Wallet", Help = "Create a new wallet")]
        public void WalletCreateCommand(string fileName)
        {
            _walletManager.CreateWallet(fileName);
            var secureString = _consoleReader.ReadPassword();
            var walletAccount = _walletManager.CreateAccount(secureString);
            
            _consoleWriter.WriteLine("");
            _consoleWriter.WriteLine("Address: " + walletAccount.Address, ConsoleOutputStyle.Information);
            _consoleWriter.WriteLine("Public Key: " + _walletManager.GetPublicKeyFromNep2(walletAccount.Key, secureString), ConsoleOutputStyle.Information);
        }

        [PromptCommand("wallet open", Category = "Wallet", Help = "Open wallet")]
        public void WalletOpenCommand(string fileName)
        {
            _walletManager.Load(fileName);
        }

        [PromptCommand("wallet close", Category = "Wallet", Help = "Close wallet")]
        public void WalletCloseCommand()
        {
            _walletManager.Close();
        }

        [PromptCommand("import wif", Category = "Wallet", Help = "Close wallet")]
        public void ImportWif(string wif)
        {
            var secureString = _consoleReader.ReadPassword();
            _walletManager.ImportWif(wif, secureString);
        }

        [PromptCommand("import nep2", Category = "Wallet", Help = "Close wallet")]
        public void ImportNep2(string nep2key)
        {
            var secureString = _consoleReader.ReadPassword();
            _walletManager.ImportEncryptedWif(nep2key, secureString);
        }

        [PromptCommand("wallet", Category = "Wallet", Help = "List all accounts from wallet")]
        public void WalletListAccountCommand(PromptOutputStyle output = PromptOutputStyle.json)
        {
            _walletManager.CheckWalletIsOpen();
            var currentWallet = _walletManager.Wallet;
            _consoleWriter.WriteObject(currentWallet, output);
        }

        [PromptCommand("wallet save", Category = "Wallet", Help = "Saves the open wallet into a new file")]
        public void WalletSaveCommand(string fileName)
        {
            _walletManager.ExportWallet(fileName);
        }

        [PromptCommand("account create", Category = "Account", Help = "Create a new account")]
        public void AccountCreateCommand()
        {
            var secureString = _consoleReader.ReadPassword();
            var walletAccount = _walletManager.CreateAccount(secureString);
            
            _consoleWriter.WriteLine("");
            _consoleWriter.WriteLine("Address: " + walletAccount.Address, ConsoleOutputStyle.Information);
            _consoleWriter.WriteLine("Public Key: " + _walletManager.GetPublicKeyFromNep2(walletAccount.Key, secureString), ConsoleOutputStyle.Information);
        }

        [PromptCommand("account delete", Category = "Account", Help = "Deletes an account")]
        public void AccountDeleteCommand(UInt160 scriptHash)
        {
            _walletManager.DeleteAccount(scriptHash);
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
