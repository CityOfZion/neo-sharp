using System.Threading.Tasks;

namespace NeoSharp.Core.Blockchain.Processors
{
    /// <summary>
    ///     A Processor reads the input and modifies the blockchain state.
    ///     During processing, no validation is done.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IProcessor<T>
    {
        Task Process(T item);
    }
}