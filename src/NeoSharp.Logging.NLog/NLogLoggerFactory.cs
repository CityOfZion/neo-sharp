using Microsoft.Extensions.Logging;
using NeoSharp.Core.Logging;
using NLog;
using NLog.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace NeoSharp.Logging.NLog
{
    public class NLogLoggerFactory : ILoggerFactoryExtended
    {
        #region Public event

        public event delOnLog OnLog;

        #endregion

        #region Private fields

        private readonly LoggerFactory _loggerFactory;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
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

        public void RaiseOnLog(LogEntry log)
        {
            OnLog?.Invoke(log);
        }

        public void AddProvider(ILoggerProvider provider)
        {
            _loggerFactory.AddProvider(provider);
        }
    }
}