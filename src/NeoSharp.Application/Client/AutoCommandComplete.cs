using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace NeoSharp.Application.Client
{
    public class AutoCommandComplete : IAutoCompleteHandler
    {
        #region Public Fields

        public readonly IReadOnlyDictionary<string, List<ParameterInfo[]>> Cache;

        public IEnumerable<string> Keys => Cache.Keys;

        public int Count => Cache.Count;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commandAutocompleteCache">Cache</param>
        public AutoCommandComplete(Dictionary<string, List<ParameterInfo[]>> commandAutocompleteCache)
        {
            Cache = new ReadOnlyDictionary<string, List<ParameterInfo[]>>(commandAutocompleteCache);
        }
    }
}