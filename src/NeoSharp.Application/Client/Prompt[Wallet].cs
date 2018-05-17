using NeoSharp.Application.Attributes;
using System.IO;

namespace NeoSharp.Application.Client
{
    public partial class Prompt : IPrompt
    {
        [PromptCommand("wallet create", Category = "Wallet", Help = "Create a new wallet")]
        private void CreateWalletCommand(FileInfo file)
        {
            if (file.Exists)
            {
                _consoleWriter.WriteLine($"File '{file.FullName}' already exist, please provide a new one", ConsoleOutputStyle.Error);
                return;
            }
        }

        [PromptCommand("wallet open", Category = "Wallet", Help = "Open wallet")]
        private void OpenWalletCommand(FileInfo file)
        {
            if (!file.Exists)
            {
                _consoleWriter.WriteLine($"File not found '{file.FullName}'", ConsoleOutputStyle.Error);
                return;
            }
        }
    }
}