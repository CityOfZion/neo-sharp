using System;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using Moq;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Network;
using NeoSharp.DI.SimpleInjector;

namespace NeoSharp.Core.Benchmark
{       
    public class BMPeerFactory
    {
        Func<IPeer> uub;

        [GlobalSetup]
        public void Setup()
        {
            var containerBuilder = new SimpleInjectorContainerBuilder();
            containerBuilder.RegisterInstanceCreator<IPeer, Peer>();
            containerBuilder.RegisterSingleton(ConfigureLogger);
            containerBuilder.Register(typeof(ILogger<>), typeof(LoggerAdapter<>));
            var container = containerBuilder.Build();

            uub = () => container.Resolve<IPeer>();
        }

        private static ILoggerFactory ConfigureLogger()
        {
            Mock<ILoggerFactory> mockLoggerFactory = new Mock<ILoggerFactory>();
            return mockLoggerFactory.Object;
        }

        [Benchmark]
        public void createPeers()
        {
            for (int i=0; i<10000; i++)
                uub();
        }

    }
}
