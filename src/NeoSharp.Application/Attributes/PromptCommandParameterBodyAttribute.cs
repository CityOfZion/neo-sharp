using System;

namespace NeoSharp.Application.Attributes
{
    /// <summary>
    /// This attribute is used for capture all the command line from this point
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class PromptCommandParameterBodyAttribute : Attribute
    {
        /// <summary>
        /// Convert as json
        /// </summary>
        public bool AsJson { get; set; } = false;
    }
}