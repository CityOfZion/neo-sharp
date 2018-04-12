using System;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using Moq;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Network;
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
            containerBuilder.RegisterInstanceCreator<IPeer, Peer>();
            containerBuilder.RegisterSingleton(ConfigureLogger);
            containerBuilder.Register(typeof(ILogger<>), typeof(LoggerAdapter<>));
            var container = containerBuilder.Build();

            _uub = () => container.Resolve<IPeer>();
        }

        private static ILoggerFactory ConfigureLogger()
        {
            var mockLoggerFactory = new Mock<ILoggerFactory>();
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
