using System;
using System.Threading;
using System.Threading.Tasks;

namespace NeoSharp.Core.Helpers
{
    public class AsyncDelayer : IAsyncDelayer
    {
        public Task Delay(TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            return Task.Delay(timeSpan, cancellationToken);
        }
    }
}
