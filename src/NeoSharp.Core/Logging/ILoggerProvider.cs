using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Core.Logging
{
    public interface ILoggerProvider<T>
    {
        void LogWarning(string warningMessage);
    }
}
