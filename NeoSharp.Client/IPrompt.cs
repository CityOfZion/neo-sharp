using System.Threading.Tasks;

namespace NeoSharp.Client
{
    public interface IPrompt
    {
        Task StartPrompt(string[] args);
    }
}