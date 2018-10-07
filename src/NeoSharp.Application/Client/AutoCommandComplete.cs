using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NeoSharp.Application.Attributes;
using NeoSharp.Application.Providers;

namespace NeoSharp.Application.Client
{
    public class AutoCommandComplete : IAutoCompleteHandler
    {
        #region Private Fields

        private readonly FileAutoCompleteAttribute _fileProvider;
        private readonly FileAutoCompleteAttribute _folderProvider;

        #endregion

        #region Public Fields

        public readonly IDictionary<string, List<ParameterInfo[]>> Cache;
        public IEnumerable<string> Commands => Cache.Keys;
        public int Count => Cache.Count;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public AutoCommandComplete()
        {
            Cache = new Dictionary<string, List<ParameterInfo[]>>();

            _fileProvider = new FileAutoCompleteAttribute(true);
            _folderProvider = new FileAutoCompleteAttribute(false);
        }

        public bool TryGetMethods(string command, out List<ParameterInfo[]> value)
        {
            return Cache.TryGetValue(command, out value);
        }

        public void Add(string command, List<ParameterInfo[]> ls)
        {
            Cache.Add(command, ls);
        }

        public IEnumerable<string> GetParameterValues(ParameterInfo parameter, string currentValue)
        {
            if (parameter == null) return new string[] { };

            IEnumerable<string> arr = null;

            var attr = parameter.GetCustomAttribute<AutoCompleteAttribute>();

            if (attr != null)
            {
                arr = attr.GetParameterValues(parameter, currentValue);
            }
            else if (parameter.ParameterType == typeof(bool))
            {
                arr = BoolValues();
            }
            else if (parameter.ParameterType.IsEnum)
            {
                arr = EnumValues(parameter.ParameterType);
            }
            else if (parameter.ParameterType == typeof(FileInfo))
            {
                arr = _fileProvider.GetParameterValues(parameter, currentValue);
            }
            else if (parameter.ParameterType == typeof(DirectoryInfo))
            {
                arr = _folderProvider.GetParameterValues(parameter, currentValue);
            }

            // Filter

            if (arr != null && !string.IsNullOrEmpty(currentValue))
            {
                return arr.Where(u => u.StartsWith(currentValue, StringComparison.InvariantCultureIgnoreCase)).ToArray();
            }

            return arr;
        }

        private IEnumerable<string> EnumValues(Type type)
        {
            foreach (var name in Enum.GetNames(type))
            {
                yield return name;
            }
        }

        private IEnumerable<string> BoolValues()
        {
            yield return bool.TrueString;
            yield return bool.FalseString;
        }
    }
}