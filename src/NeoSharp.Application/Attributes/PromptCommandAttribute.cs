using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace NeoSharp.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PromptCommandAttribute : Attribute
    {
        #region Constants

        static Type _StringType = typeof(string);
        static Type _IListType = typeof(IList);
        static Type _FileInfoType = typeof(FileInfo);
        static Type _DirectoryInfoType = typeof(DirectoryInfo);

        static readonly char[] SplitChars = new char[] { ';', ',', '|' };

        #endregion

        #region Variables

        ParameterInfo[] _Parameters;
        MethodInfo _Method;

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
            get { return _Method; }
            set
            {
                if (value == null) return;

                _Method = value;
                _Parameters = value.GetParameters();
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
            int max = _Parameters.Length;
            object[] ret = new object[max];

            if (args.Length != max)
                throw (new ArgumentException("Missing parameters"));

            for (int x = 0; x < max; x++)
            {
                ret[x] = ParseToArgument(args[x], _Parameters[x].ParameterType);
            }

            return ret;
        }

        /// <summary>
        /// Parse argument
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="type">Type</param>
        /// <returns>Return parsed argument</returns>
        object ParseToArgument(string input, Type type)
        {
            // FileInfo
            if (_FileInfoType == type)
            {
                return new FileInfo(input);
            }

            // DirectoryInfo
            if (_DirectoryInfoType == type)
            {
                return new DirectoryInfo(input);
            }

            // Array
            if (type.IsArray)
            {
                List<object> l = new List<object>();
                Type gt = type.GetElementType();
                foreach (string ii in input.Split(SplitChars))
                {
                    object ov = ParseToArgument(ii, gt);
                    if (ov == null) continue;

                    l.Add(ov);
                }

                Array a = (Array)Activator.CreateInstance(type, l.Count);
                Array.Copy(l.ToArray(), a, l.Count);
                return a;
            }

            // List
            if (_IListType.IsAssignableFrom(type))
            {
                IList l = (IList)Activator.CreateInstance(type);

                // If dosen't have T return null
                if (type.GenericTypeArguments == null || type.GenericTypeArguments.Length == 0)
                    return null;

                Type gt = type.GenericTypeArguments[0];
                foreach (string ii in input.Split(SplitChars))
                {
                    object ov = ParseToArgument(ii, gt);
                    if (ov == null) continue;

                    l.Add(ov);
                }
                return l;
            }

            // Is Convertible
            TypeConverter conv = TypeDescriptor.GetConverter(type);
            if (conv.CanConvertFrom(_StringType))
            {
                return conv.ConvertFrom(input);
            }

            throw (new ArgumentException());
        }
    }
}