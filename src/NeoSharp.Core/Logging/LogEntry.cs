using System;
using Microsoft.Extensions.Logging;

namespace NeoSharp.Core.Logging
{
    public class LogEntry
    {
        /// <summary>
        /// Level
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// Category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Exception
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Message with exception
        /// </summary>
        public string MessageWithError
        {
            get
            {
                if (string.IsNullOrEmpty(Message))
                {
                    return Exception == null ? "" : Exception.ToString();
                }

                if (Exception == null)
                {
                    return Message;
                }

                return Message + Environment.NewLine + Exception.ToString();
            }
        }
    }
}