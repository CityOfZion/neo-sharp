using System;
using NeoSharp.Core.DI;
using NeoSharp.Core.Wallet;
using NeoSharp.Core.Wallet.NEP6;

namespace NeoSharp.Application.DI
{
    public class WalletModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterSingleton<IWalletManager, Nep6WalletManager>();
        }
    }
}
