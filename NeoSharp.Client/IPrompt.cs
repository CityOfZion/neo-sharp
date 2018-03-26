using System.Threading.Tasks;

namespace NeoSharp.Client
{
    public interface IPrompt
    {
        void StartPrompt(string[] args);
    }
}