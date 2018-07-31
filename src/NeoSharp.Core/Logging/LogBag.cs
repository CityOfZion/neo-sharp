using System.Collections.Concurrent;

namespace NeoSharp.Core.Logging
{
    public class LogBag : ConcurrentBag<LogEntry>, ILogBag
    {

    }
}