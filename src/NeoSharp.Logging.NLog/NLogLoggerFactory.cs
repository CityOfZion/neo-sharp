using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace NeoSharp.Logging.NLog
{
    public class NLogLoggerFactory : ILoggerFactory
    {
        private readonly LoggerFactory _loggerFactory;

        public NLogLoggerFactory()
        {
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddNLog(new NLogProviderOptions
            {
                CaptureMessageTemplates = true,
                CaptureMessageProperties = true
            });
            LogManager.LoadConfiguration("nlog.config");
        }

        public void Dispose()
        {
            _loggerFactory.Dispose();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggerFactory.CreateLogger(categoryName);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            _loggerFactory.AddProvider(provider);
        }
    }
}