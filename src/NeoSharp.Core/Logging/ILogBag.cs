using System.Collections.Generic;

namespace NeoSharp.Core.Logging
{
    public interface ILogBag : IEnumerable<LogEntry>
    {
        int Count { get; }

        bool IsEmpty { get; }

        void Add(LogEntry item);

        void Clear();

        bool TryPeek(out LogEntry result);

        bool TryTake(out LogEntry result);
    }
}