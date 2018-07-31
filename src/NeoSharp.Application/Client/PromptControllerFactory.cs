using System;
using System.Collections;
using System.Collections.Generic;

namespace NeoSharp.Application.Client
{
    public class PromptControllerFactory : IEnumerable<Type>
    {
        #region Private fields

        Type[] _controllers;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="controllers">Controllers</param>
        public PromptControllerFactory(params Type[] controllers)
        {
            _controllers = controllers;
        }

        public IEnumerator<Type> GetEnumerator()
        {
            foreach (var c in _controllers) yield return c;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var c in _controllers) yield return c;
        }
    }
}