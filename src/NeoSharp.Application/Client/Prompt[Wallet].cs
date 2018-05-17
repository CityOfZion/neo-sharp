using NeoSharp.Application.Attributes;
using System.IO;

namespace NeoSharp.Application.Client
{
    public partial class Prompt : IPrompt
    {
        [PromptCommand("wallet create", Category = "Wallet", Help = "Create a new wallet")]
        private void WalletCreateCommand(FileInfo file)
        {
            if (file.Exists)
            {
                _consoleWriter.WriteLine($"File '{file.FullName}' already exist, please provide a new one", ConsoleOutputStyle.Error);
                return;
            }
        }

        [PromptCommand("wallet open", Category = "Wallet", Help = "Open wallet")]
        private void WalletOpenCommand(FileInfo file)
        {
            if (!file.Exists)
            {
                _consoleWriter.WriteLine($"File not found '{file.FullName}'", ConsoleOutputStyle.Error);
                return;
            }
        }

        [PromptCommand("wallet close", Category = "Wallet", Help = "Close wallet")]
        private void WalletCloseCommand()
        {

        }

        /*
        TODO:
        wallet {verbose}
        wallet claim
        wallet migrate
        wallet rebuild {start block}
        wallet delete_addr {addr}
        wallet delete_token {token_contract_hash}
        wallet alias {addr} {title}
        wallet tkn_send {token symbol} {address_from} {address to} {amount} 
        wallet tkn_send_from {token symbol} {address_from} {address to} {amount}
        wallet tkn_approve {token symbol} {address_from} {address to} {amount}
        wallet tkn_allowance {token symbol} {address_from} {address to}
        wallet tkn_mint {token symbol} {mint_to_addr} (--attach-neo={amount}, --attach-gas={amount})
        wallet tkn_register {addr} ({addr}...) (--from-addr={addr})
        wallet unspent

        import wif {wif}
        import nep2 {nep2_encrypted_key}
        import multisig_addr {pubkey in wallet} {minimum # of signatures required} {signing pubkey 1} {signing pubkey 2}...
        import watch_addr {address}
        import token {token_contract_hash}
        export wif {address}
        export nep2 {address}

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