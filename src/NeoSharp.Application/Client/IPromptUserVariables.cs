using System.Collections.Generic;

namespace NeoSharp.Application.Client
{
    public interface IPromptUserVariables : IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>
        /// Replace the variables inside the input
        /// </summary>
        /// <param name="input">Input</param>
        /// <returns>Return replaced input</returns>
        string Replace(string input);

        /// <summary>
        /// Add a new variable
        /// </summary>
        /// <param name="varName">Name</param>
        /// <param name="value">Value</param>
        void Add(string varName, string value);

        /// <summary>
        /// Remove a variable
        /// </summary>
        /// <param name="varName">Variable</param>
        /// <returns>Return true if remove</returns>
        bool Remove(string varName);
    }
}