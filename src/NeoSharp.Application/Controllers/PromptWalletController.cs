using System.IO;
using System.Linq;
using NeoSharp.Application.Attributes;
using NeoSharp.Application.Client;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Wallet;
using NeoSharp.Core.Wallet.Exceptions;
using NeoSharp.Types;
using NeoSharp.Types.ExtensionMethods;

namespace NeoSharp.Application.Controllers
{
    public class PromptWalletController : IPromptController
    {
        #region Private fields

        /// <summary>
        /// The wallet.
        /// </summary>
        private readonly IWalletManager _walletManager;
        private readonly IConsoleHandler _consoleHandler;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="walletManager">Wallet manager</param>
        /// <param name="consoleHandler">Console handler</param>
        public PromptWalletController(IWalletManager walletManager, IConsoleHandler consoleHandler)
        {
            _walletManager = walletManager;
            _consoleHandler = consoleHandler;
        }

        [PromptCommand("wallet create", Category = "Wallet", Help = "Create a new wallet")]
        public void WalletCreateCommand(FileInfo file)
        {
            var secureString = _consoleHandler.ReadPassword();
            _consoleHandler.ApplyStyle(ConsoleOutputStyle.Prompt);
            var confirmationString = _consoleHandler.ReadPassword("\nConfirm your password:");
            if (secureString.ToByteArray().SequenceEqual(confirmationString.ToByteArray()))
            {
                _walletManager.CreateWallet(file.FullName);
                var walletAccount = _walletManager.CreateAndAddAccount(secureString);
                _consoleHandler.ApplyStyle(ConsoleOutputStyle.Prompt);
                _consoleHandler.WriteLine("\nAddress: " + walletAccount.Address, ConsoleOutputStyle.Information);
                _consoleHandler.WriteLine("Public Key: " + _walletManager.GetPublicKeyFromNep2(walletAccount.Key, secureString), ConsoleOutputStyle.Information);
            }
            else
            {
                _consoleHandler.WriteLine("\nPasswords don't match.", ConsoleOutputStyle.Information);
            }
        }

        [PromptCommand("wallet open", Category = "Wallet", Help = "Open wallet")]
        public void WalletOpenCommand(FileInfo file)
        {
            _walletManager.Load(file.FullName);
        }

        [PromptCommand("wallet close", Category = "Wallet", Help = "Close wallet")]
        public void WalletCloseCommand()
        {
            _walletManager.Close();
        }

        [PromptCommand("import wif", Category = "Wallet", Help = "Close wallet")]
        public void ImportWif(string wif)
        {
            var secureString = _consoleHandler.ReadPassword();
            _walletManager.ImportWif(wif, secureString);
        }

        [PromptCommand("import nep2", Category = "Wallet", Help = "Close wallet")]
        public void ImportNep2(string nep2key)
        {
            var secureString = _consoleHandler.ReadPassword();
            _walletManager.ImportEncryptedWif(nep2key, secureString);
        }

        [PromptCommand("wallet list", Category = "Wallet", Help = "List all accounts from wallet")]
        public void WalletListAccountCommand(PromptOutputStyle output = PromptOutputStyle.json)
        {
            _walletManager.CheckWalletIsOpen();
            var currentWallet = _walletManager.Wallet;
            _consoleHandler.WriteObject(currentWallet, output);
        }

        [PromptCommand("wallet save", Category = "Wallet", Help = "Saves the open wallet into a new file")]
        public void WalletSaveCommand(FileInfo file)
        {
            _walletManager.ExportWallet(file.FullName);
        }

        [PromptCommand("account create", Category = "Account", Help = "Create a new account")]
        public void AccountCreateCommand()
        {
            var secureString = _consoleHandler.ReadPassword("Wallet password:");
            try
            {
                _walletManager.CheckIfPasswordMatchesOpenWallet(secureString);
                _consoleHandler.ApplyStyle(ConsoleOutputStyle.Prompt);
                var walletAccount = _walletManager.CreateAndAddAccount(secureString);
                _consoleHandler.WriteLine("\nAddress: " + walletAccount.Address, ConsoleOutputStyle.Information);
                _consoleHandler.WriteLine("Public Key: " + _walletManager.GetPublicKeyFromNep2(walletAccount.Key, secureString), ConsoleOutputStyle.Information);
            }
            catch (AccountsPasswordMismatchException)
            {
                _consoleHandler.WriteLine("\nInvalid password.");
            }
        }

        [PromptCommand("account delete", Category = "Account", Help = "Deletes an account")]
        public void AccountDeleteCommand(string address)
        {
            //Should we ask for a confirmation? Should we ask for the password?
            _walletManager.DeleteAccount(address.ToScriptHash());
            _consoleHandler.WriteLine("Account deleted.");
        }

        [PromptCommand("account export nep2", Category = "Account", Help = "Exports an account in nep-2 format")]
        public void AccountExportNep2(string address)
        {
            var walletAccount = _walletManager.GetAccount(address.ToScriptHash());
            if (walletAccount != null)
            {
                try
                {
                    var walletPassword = _consoleHandler.ReadPassword();
                    byte[] accountPrivateKey = _walletManager.DecryptNep2(walletAccount.Key, walletPassword);
                    var newKeyPassword = _consoleHandler.ReadPassword("\nNew key password:");
                    var newKeyPasswordConfirmation = _consoleHandler.ReadPassword("\nConfirm your password:");
                    if (newKeyPassword.ToByteArray().SequenceEqual(newKeyPasswordConfirmation.ToByteArray()))
                    {
                        string nep2Key = _walletManager.EncryptNep2(accountPrivateKey, newKeyPassword);
                        _consoleHandler.WriteLine("\nExported NEP-2 Key: " + nep2Key);
                    }
                    else
                    {
                        _consoleHandler.WriteLine("\nPasswords don't match.");
                    }
                }
                catch (AccountsPasswordMismatchException)
                {
                    _consoleHandler.WriteLine("\nInvalid password.");
                }

            }
            else
            {
                _consoleHandler.WriteLine("\nAccount not found.");
            }

        }

        [PromptCommand("account export wif", Category = "Account", Help = "Exports an account in nep-2 format")]
        public void AccountExportWif(string address)
        {
            var walletAccount = _walletManager.GetAccount(address.ToScriptHash());
            if (walletAccount != null)
            {
                try
                {
                    var walletPassword = _consoleHandler.ReadPassword();
                    byte[] accountPrivateKey = _walletManager.DecryptNep2(walletAccount.Key, walletPassword);
                    string wif = _walletManager.PrivateKeyToWif(accountPrivateKey);
                    _consoleHandler.WriteLine("\nExported wif: " + wif);
                }
                catch (AccountsPasswordMismatchException)
                {
                    _consoleHandler.WriteLine("\nInvalid password.");
                }
            }
            else
            {
                _consoleHandler.WriteLine("\nAccount not found.");
            }
        }

        [PromptCommand("account alias", Category = "Account", Help = "Adds a label to an account")]
        public void AddAccountAlias(string address, string alias)
        {
            UInt160 accountScriptHash = address.ToScriptHash();
            if (_walletManager.Contains(accountScriptHash))
            {
                _walletManager.UpdateAccountAlias(accountScriptHash, alias);
            }
            else
            {
                _consoleHandler.WriteLine("\nAccount not found.");
            }
        }


        /*
        TODO #404: Implement additional wallet features
        wallet delete_token {token_contract_hash}
        wallet alias {addr} {title}

        import multisig_addr {pubkey in wallet} {minimum # of signatures required} {signing pubkey 1} {signing pubkey 2}...
        import watch_addr {address}
        import token {token_contract_hash}
         */

        /*
        TODO #405: Implement additional transaction manager features
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