using System.Threading.Tasks;
using NeoSharp.Application.DI;
using NeoSharp.Core;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.DI;
using NeoSharp.Core.Models;
using NeoSharp.DI.SimpleInjector;

namespace NeoSharp.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var containerBuilder = new SimpleInjectorContainerBuilder();

            containerBuilder.RegisterModule<CoreModule>();
            containerBuilder.RegisterModule<ConfigurationModule>();
            containerBuilder.RegisterModule<LoggingModule>();
            containerBuilder.RegisterModule<SerializationModule>();
            containerBuilder.RegisterModule<PersistenceModule>();
            containerBuilder.RegisterModule<ClientModule>();
            containerBuilder.RegisterModule<WalletModule>();
            containerBuilder.RegisterModule<VMModule>();

            var container = containerBuilder.Build();

            // FixDb(container).Wait();

            var bootstrapper = container.Resolve<IBootstrapper>();

            bootstrapper.Start(args);
        }

        private static async Task FixDb(IContainer container)
        {
            var blockRepository = container.Resolve<IBlockRepository>();

            await blockRepository.SetTotalBlockHeight(1_271_699U);

            for (var corruptedBlockHeight = 1_271_700U; corruptedBlockHeight < 1_271_800U; corruptedBlockHeight++)
            {
                var corruptedBlockHeader = await blockRepository.GetBlockHeader(corruptedBlockHeight);

                if (corruptedBlockHeader == null)
                {
                    break;
                }

                if (corruptedBlockHeader.Type == HeaderType.Header)
                {
                    continue;
                }

                corruptedBlockHeader.Type = HeaderType.Header;

                await blockRepository.UpdateBlockHeader(corruptedBlockHeader);
            }
        }
    }
}