using System.Collections.Generic;

namespace NeoSharp.Application.Client
{
    public class PromptUserVariables : Dictionary<string, string>, IPromptUserVariables
    {
        /// <summary>
        /// Replace the variables inside the input
        /// </summary>
        /// <param name="input">Input</param>
        /// <returns>Return replaced input</returns>
        public virtual string Replace(string input)
        {
            foreach (var keyValue in this)
            {
                input = input.Replace($"${keyValue.Key}", keyValue.Value);
            }

            return input;
        }
    }
}