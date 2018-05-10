using System;

namespace NeoSharp.Application.Attributes
{
    /// <summary>
    /// This attribute is used for capture all the command line from this point
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    class PromptCommandParameterBodyAttribute : Attribute
    {

    }
}