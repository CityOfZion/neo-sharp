using System;
using System.Threading;
using System.Threading.Tasks;

namespace NeoSharp.Core.Helpers
{
    public interface IAsyncDelayer
    {
        Task Delay(TimeSpan timeSpan, CancellationToken cancellationToken);
    }
}
