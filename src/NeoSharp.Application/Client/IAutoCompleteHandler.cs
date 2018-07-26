using System.Collections.Generic;

namespace NeoSharp.Application.Client
{
    public interface IAutoCompleteHandler
    {
        /// <summary>
        /// Keys
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Count
        /// </summary>
        int Count { get; }
    }
}