using Moq;
using NeoSharp.Application.Client;

namespace NeoSharp.Application.Test.Extensions
{
    public static class ConsoleReaderMockExtensions
    {
        public static Mock<IConsoleHandler> SetupStringCommandReader(this Mock<IConsoleHandler> consoleReaderMock, string cmd)
        {
            consoleReaderMock
                .SetupSequence(x => x.ReadFromConsole(null))
                .Returns(cmd)
                .Returns("exit");

            return consoleReaderMock;
        }
    }
}