using System.Collections.Generic;
using System.Reflection;

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

        /// <summary>
        /// Try get value
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="value">Value</param>
        /// <returns>Return true if is found</returns>
        bool TryGetValue(string command, out List<ParameterInfo[]> value);

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="ls">List</param>
        void Add(string command, List<ParameterInfo[]> ls);
    }
}