using System;

namespace NeoSharp.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class PromptHideHelpCommandAttribute : Attribute { }
}