using System;
using System.Collections.Generic;
using System.Reflection;

namespace NeoSharp.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class AutoCompleteAttribute : Attribute
    {
        public abstract IEnumerable<string> GetParameterValues(ParameterInfo parameter, string currentValue);
    }
}