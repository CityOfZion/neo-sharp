namespace NeoSharp.Application.Client
{
    public interface IConsoleWriter
    {
        void Write(string output);
        void WriteLine(string output);
    }
}