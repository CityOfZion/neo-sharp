namespace NeoSharp.Core.Logging
{
    public interface ILoggerProvider<T>
    {
        void LogWarning(string warningMessage);
    }
}