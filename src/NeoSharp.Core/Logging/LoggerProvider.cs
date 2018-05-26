using Microsoft.Extensions.Logging;

namespace NeoSharp.Core.Logging
{
    public class LoggerProvider<T> : ILoggerProvider<T>
    {
        private readonly ILogger<T> _logger;

        public LoggerProvider(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogWarning(string warningMessage)
        {
            _logger.LogWarning(warningMessage);
        }
    }
}
