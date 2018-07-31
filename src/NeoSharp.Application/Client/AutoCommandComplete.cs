using System.Collections.Generic;
using System.Reflection;

namespace NeoSharp.Application.Client
{
    public class AutoCommandComplete : IAutoCompleteHandler
    {
        #region Public Fields

        public readonly IDictionary<string, List<ParameterInfo[]>> Cache;

        public IEnumerable<string> Keys => Cache.Keys;

        public int Count => Cache.Count;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public AutoCommandComplete()
        {
            Cache = new Dictionary<string, List<ParameterInfo[]>>();
        }

        public bool TryGetValue(string command, out List<ParameterInfo[]> value)
        {
            return Cache.TryGetValue(command, out value);
        }

        public void Add(string command, List<ParameterInfo[]> ls)
        {
            Cache.Add(command, ls);
        }
    }
}