using System.Threading.Tasks;

namespace NeoSharp.Core.Blockchain.Processing
{
    /// <summary>
    ///     A persister reads the input and modifies the blockchain state.
    ///     During processing, no validation is done.
    /// </summary>
    /// <typeparam name="TTransaction"></typeparam>
    public interface ITransactionPersister<in TTransaction>
    {
        Task Persist(TTransaction transaction);
    }
}