using System;
using BenchmarkDotNet.Attributes;
using Moq;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Network;
using NeoSharp.Core.Network.Tcp;
using NeoSharp.DI.SimpleInjector;

namespace NeoSharp.Core.Benchmark
{       
    public class BmPeerFactory
    {
        Func<IPeer> _uub;

        [GlobalSetup]
        public void Setup()
        {
            var containerBuilder = new SimpleInjectorContainerBuilder();
            containerBuilder.RegisterInstanceCreator<IPeer, TcpPeer>();
            containerBuilder.RegisterSingleton(ConfigureLogger);
            containerBuilder.Register(typeof(ILogger<>), typeof(LoggerAdapter<>));
            var container = containerBuilder.Build();

            _uub = () => container.Resolve<IPeer>();
        }

        private static Microsoft.Extensions.Logging.ILoggerFactory ConfigureLogger()
        {
            var mockLoggerFactory = new Mock<Microsoft.Extensions.Logging.ILoggerFactory>();
            return mockLoggerFactory.Object;
        }

        [Benchmark]
        public void CreatePeers()
        {
            for (var i=0; i<10000; i++)
                _uub();
        }

    }
}
