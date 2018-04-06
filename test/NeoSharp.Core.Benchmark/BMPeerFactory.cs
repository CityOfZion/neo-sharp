using System;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using Moq;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Network;
using SimpleInjector;

namespace NeoSharp.Core.Benchmark
{       
    public class BMPeerFactory
    {
        Func<IPeer> uub;

        [GlobalSetup]
        public void Setup()
        {
            Container cont = new Container();
            cont.Register<IPeer, Peer>(Lifestyle.Transient);
            cont.Register(ConfigureLogger, Lifestyle.Singleton);
            cont.Register(typeof(ILogger<>), typeof(LoggerAdapter<>));
            uub = () => cont.GetInstance<IPeer>();
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
