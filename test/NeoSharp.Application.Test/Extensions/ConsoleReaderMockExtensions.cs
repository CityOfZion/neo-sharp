using System.Threading;
using Moq;
using NeoSharp.Application.Client;

namespace NeoSharp.Application.Test.Extensions
{
    public static class ConsoleReaderMockExtensions
    {
        public static Mock<IConsoleReader> SetupStringCommandReader(this Mock<IConsoleReader> consoleReaderMock, string cmd)
        {
            var cancel = new CancellationTokenSource();

            consoleReaderMock
                .SetupSequence(x => x.ReadFromConsole(cancel.Token, null))
                .Returns(cmd)
                .Returns("exit");

            return consoleReaderMock;
        }
    }
}