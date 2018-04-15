using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace NeoSharp.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PromptCommandAttribute : Attribute
    {
        #region Constants

        private static readonly Type _stringType = typeof(string);
        private static readonly Type _iListType = typeof(IList);
        private static readonly Type _fileInfoType = typeof(FileInfo);
        private static readonly Type _directoryInfoType = typeof(DirectoryInfo);

        private static readonly char[] _splitChars = { ';', ',', '|' };

        #endregion

        #region Variables

        private ParameterInfo[] _parameters;
        private MethodInfo _method;

        #endregion

        #region Properties

        /// <summary>
        /// Commands
        /// </summary>
        public readonly string[] Commands;
        /// <summary>
        /// Help
        /// </summary>
        public string Help { get; set; }
        /// <summary>
        /// Method
        /// </summary>
        internal MethodInfo Method
        {
            get { return _method; }
            set
            {
                if (value == null) return;

                _method = value;
                _parameters = value.GetParameters();
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commands">Commands</param>
        public PromptCommandAttribute(params string[] commands)
        {
            Commands = commands;
        }

        /// <summary>
        /// Convert string arguments to Method arguments
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <returns>Return parsed arguments</returns>
        public object[] ConvertToArguments(string[] args)
        {
            var max = _parameters.Length;
            var ret = new object[max];

            if (args.Length != max)
                throw (new ArgumentException("Missing parameters"));

            for (var x = 0; x < max; x++)
            {
                ret[x] = ParseToArgument(args[x], _parameters[x].ParameterType);
            }

            return ret;
        }

        /// <summary>
        /// Parse argument
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="type">Type</param>
        /// <returns>Return parsed argument</returns>
        private object ParseToArgument(string input, Type type)
        {
            // FileInfo
            if (_fileInfoType == type)
            {
                return new FileInfo(input);
            }

            // DirectoryInfo
            if (_directoryInfoType == type)
            {
                return new DirectoryInfo(input);
            }

            // Array
            if (type.IsArray)
            {
                var l = new List<object>();
                var gt = type.GetElementType();
                foreach (var ii in input.Split(_splitChars))
                {
                    var ov = ParseToArgument(ii, gt);
                    if (ov == null) continue;

                    l.Add(ov);
                }

                var a = (Array)Activator.CreateInstance(type, l.Count);
                Array.Copy(l.ToArray(), a, l.Count);
                return a;
            }

            // List
            if (_iListType.IsAssignableFrom(type))
            {
                var l = (IList)Activator.CreateInstance(type);

                // If dosen't have T return null
                if (type.GenericTypeArguments == null || type.GenericTypeArguments.Length == 0)
                    return null;

                var gt = type.GenericTypeArguments[0];
                foreach (var ii in input.Split(_splitChars))
                {
                    var ov = ParseToArgument(ii, gt);
                    if (ov == null) continue;

                    l.Add(ov);
                }
                return l;
            }

            // Is Convertible
            var conv = TypeDescriptor.GetConverter(type);
            if (conv.CanConvertFrom(_stringType))
            {
                return conv.ConvertFrom(input);
            }

            throw (new ArgumentException());
        }
    }
}