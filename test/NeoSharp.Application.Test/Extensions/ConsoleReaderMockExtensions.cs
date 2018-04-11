using Moq;
using NeoSharp.Application.Client;

namespace NeoSharp.Application.Test.Extensions
{
    public static class ConsoleReaderMockExtensions
    {
        public static Mock<IConsoleReader> SetupStringCommandReader(this Mock<IConsoleReader> consoleReaderMock, string cmd)
        {
            consoleReaderMock
                .SetupSequence(x => x.ReadFromConsole())
                .Returns(cmd)
                .Returns("exit");

            return consoleReaderMock;
        }
    }
}
