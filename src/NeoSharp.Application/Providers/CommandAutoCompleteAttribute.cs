using System.Collections.Generic;
using System.Reflection;
using NeoSharp.Application.Attributes;

namespace NeoSharp.Application.Providers
{
    public class CommandAutoCompleteAttribute : AutoCompleteAttribute
    {
        /// <summary>
        /// Available command
        /// </summary>
        public static string[] Availables { get; internal set; }

        public override IEnumerable<string> GetParameterValues(ParameterInfo parameter, string currentValue)
        {
            return Availables;
        }
    }
}