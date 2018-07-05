using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;

namespace NeoSharp.Core.Logging
{
    public class LoggerAdapter<TCategory> : ILogger<TCategory>
    {
        #region Privte fields

        private readonly ILoggerFactoryExtended _factory;
        private readonly ILogger _logger;

        #endregion

        public LoggerAdapter(ILoggerFactoryExtended factory)
        {
            _factory = factory;
            _logger = factory.CreateLogger<TCategory>();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        //------------------------------------------DEBUG------------------------------------------//

        /// <inheritdoc />
        public void LogDebug(Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Debug, exception, message, args);
        }

        /// <inheritdoc />
        public void LogDebug(string message, params object[] args)
        {
            Log(LogLevel.Debug, message, args);
        }

        //------------------------------------------TRACE------------------------------------------//

        /// <inheritdoc />
        public void LogTrace(Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Trace, exception, message, args);
        }

        /// <inheritdoc />
        public void LogTrace(string message, params object[] args)
        {
            Log(LogLevel.Trace, message, args);
        }

        //------------------------------------------INFORMATION------------------------------------------//

        /// <inheritdoc />
        public void LogInformation(Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Information, exception, message, args);
        }

        /// <inheritdoc />
        public void LogInformation(string message, params object[] args)
        {
            Log(LogLevel.Information, message, args);
        }

        //------------------------------------------WARNING------------------------------------------//

        /// <inheritdoc />
        public void LogWarning(Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Warning, exception, message, args);
        }

        /// <inheritdoc />
        public void LogWarning(string message, params object[] args)
        {
            Log(LogLevel.Warning, message, args);
        }

        //------------------------------------------ERROR------------------------------------------//

        /// <inheritdoc />
        public void LogError(Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Error, exception, message, args);
        }

        /// <inheritdoc />
        public void LogError(string message, params object[] args)
        {
            Log(LogLevel.Error, message, args);
        }

        //------------------------------------------CRITICAL------------------------------------------//

        /// <inheritdoc />
        public void LogCritical(Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Critical, exception, message, args);
        }

        /// <inheritdoc />
        public void LogCritical(string message, params object[] args)
        {
            Log(LogLevel.Critical, message, args);
        }

        /// <inheritdoc />
        public void Log(LogLevel logLevel, string message, params object[] args)
        {
            Log(logLevel, null, message, args);
        }

        /// <inheritdoc />
        public void Log(LogLevel logLevel, Exception exception, string message, params object[] args)
        {
            _factory?.RaiseOnLog(new LogEntry()
            {
                Category = typeof(TCategory).Name,
                Level = logLevel,
                Message = message,
                Exception = exception
            });

            _logger.Log(logLevel, 0, new FormattedLogValues(message, args), exception, MessageFormatter);
        }

        //------------------------------------------Scope------------------------------------------//

        /// <inheritdoc />
        public IDisposable BeginScope(string messageFormat, params object[] args)
        {
            return _logger.BeginScope(new FormattedLogValues(messageFormat, args));
        }

        //------------------------------------------HELPERS------------------------------------------//

        private static string MessageFormatter(object state, Exception error)
        {
            return state.ToString();
        }
    }
}