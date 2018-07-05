using Microsoft.Extensions.Logging;

namespace NeoSharp.Core.Logging
{
    public delegate void delOnLog(LogEntry log);

    public interface ILoggerFactoryExtended : ILoggerFactory
    {
        event delOnLog OnLog;

        void RaiseOnLog(LogEntry log);
    }
}