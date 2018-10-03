using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection;
using NeoSharp.Core.DI;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;
using NeoSharp.Types;
using NeoSharp.Types.ExtensionMethods;
using Newtonsoft.Json;

namespace NeoSharp.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PromptCommandAttribute : Attribute
    {
        #region Constants

        private static readonly Type _stringType = typeof(string);
        private static readonly Type _iListType = typeof(IList);
        private static readonly Dictionary<Type, Func<CommandToken, object>> _customConvertes = new Dictionary<Type, Func<CommandToken, object>>();

        private static readonly char[] _splitChars = { ';', ',', '|' };

        /// <summary>
        /// Static constructor
        /// </summary>
        static PromptCommandAttribute()
        {
            _customConvertes[typeof(object[])] = (token) => ParseObjectFromString(token.Value);
            _customConvertes[typeof(byte[])] = (token) => token.Value.HexToBytes();
            _customConvertes[typeof(FileInfo)] = (token) => new FileInfo(token.Value);
            _customConvertes[typeof(DirectoryInfo)] = (token) => new DirectoryInfo(token.Value);
            _customConvertes[typeof(IPAddress)] = (token) =>
            {
                var ip = token.Value;
                var uriType = Uri.CheckHostName(ip);

                if (uriType == UriHostNameType.Dns)
                {
                    // Check dns
                    var hostEntry = Dns.GetHostEntry(ip);
                    if (hostEntry.AddressList.Length == 0) throw (new ArgumentException(nameof(IPAddress)));
                    ip = hostEntry.AddressList.FirstOrDefault().ToString();
                }

                return IPAddress.Parse(ip);
            };
            _customConvertes[typeof(IPEndPoint)] = (token) =>
            {
                var split = token.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (split.Length != 2) throw (new ArgumentException(nameof(IPEndPoint)));

                var uriType = Uri.CheckHostName(split[0]);
                if (uriType == UriHostNameType.Dns)
                {
                    // Check dns
                    var hostEntry = Dns.GetHostEntry(split[0]);
                    if (hostEntry.AddressList.Length == 0) throw (new ArgumentException(nameof(IPAddress)));
                    split[0] = hostEntry.AddressList.FirstOrDefault().ToString();
                }

                var ip = IPAddress.Parse(split[0]);
                var port = ushort.Parse(split[1]);

                return new IPEndPoint(ip, port);
            };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Instance
        /// </summary>
        public object Instance;
        /// <summary>
        /// Command
        /// </summary>
        public readonly string Command;
        /// <summary>
        /// Commands
        /// </summary>
        public readonly string[] Commands;
        /// <summary>
        /// Command Length
        /// </summary>
        public readonly int TokensCount;
        /// <summary>
        /// Help
        /// </summary>
        public string Help { get; set; }
        /// <summary>
        /// Category
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Order
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Parameters
        /// </summary>
        public ParameterInfo[] Parameters { get; private set; }
        /// <summary>
        /// Method
        /// </summary>
        public MethodInfo Method { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command">Command</param>
        public PromptCommandAttribute(string command)
        {
            Command = command.ToLowerInvariant();
            Commands = Command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            TokensCount = Commands.Length;
        }

        /// <summary>
        /// Set Method
        /// </summary>
        /// <param name="instance">Instance</param>
        /// <param name="method">Method</param>
        internal void SetMethod(object instance, MethodInfo method)
        {
            if (method == null) return;

            Instance = instance;
            Method = method;
            Parameters = method.GetParameters();
        }

        /// <summary>
        /// Convert string arguments to Method arguments
        /// </summary>
        /// <param name="tokens">Command tokens</param>
        /// <param name="injector">Injector</param>
        /// <returns>Return parsed arguments</returns>
        public object[] ConvertToArguments(CommandToken[] tokens, IContainer injector)
        {
            var maxPars = Parameters.Length;
            var ret = new object[maxPars];
            var injected = new List<int>();

            // Fill default parameters

            for (var x = 0; x < ret.Length; x++)
            {
                if (Parameters[x].HasDefaultValue)
                {
                    ret[x] = Parameters[x].DefaultValue;
                }
                else
                {
                    if (injector != null && injector.TryResolve(Parameters[x].ParameterType, out var obj))
                    {
                        ret[x] = obj;
                        injected.Add(x);
                    }
                }
            }

            // Fill argument values

            for (int parameterIndex = 0, tokenIndex = 0; parameterIndex < maxPars && tokenIndex < tokens.Length; parameterIndex++, tokenIndex++)
            {
                if (injected.Contains(parameterIndex))
                {
                    tokenIndex--;
                    continue;
                }

                var body = Parameters[parameterIndex].GetCustomAttribute<PromptCommandParameterBodyAttribute>();

                if (body != null)
                {
                    // From here to the end

                    var join = string.Join(" ", tokens.Skip(tokenIndex));


                    if (body.FromJson)
                    {
                        join = join.Trim();

                        if (Parameters[parameterIndex].ParameterType.IsArray && !(join.StartsWith("[") && join.EndsWith("]")))
                        {
                            // Is an array but only one object is given
                            join = $"[{join}]";
                        }

                        ret[parameterIndex] = JsonConvert.DeserializeObject(join, Parameters[parameterIndex].ParameterType);
                    }
                    else
                    {
                        ret[parameterIndex] = ParseToArgument(new CommandToken(join, false), Parameters[parameterIndex].ParameterType);
                    }

                    return ret;
                }
                else
                {
                    // Regular parameter

                    ret[parameterIndex] = ParseToArgument(tokens[tokenIndex], Parameters[parameterIndex].ParameterType);
                }
            }

            // Check null values

            for (var x = 0; x < maxPars; x++)
            {
                if (!Parameters[x].HasDefaultValue && ret[x] == null)
                {
                    throw (new Exception($"Missing parameter value <{Parameters[x].Name}>"));
                }
            }

            return ret;
        }

        /// <summary>
        /// Here we don't know the type that we should return, and we need to create the best one
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>Return object</returns>
        static object ParseObjectFromToken(CommandToken token)
        {
            if (!token.Quoted)
            {
                if (token.Value.StartsWith("0x"))
                {
                    return token.Value.HexToBytes();
                }

                // Number?

                if (BigInteger.TryParse(token.Value, out var bi)) return bi;

                // Decimal?

                if (BigDecimal.TryParse(token.Value, 20, out var bd)) return bd;

                // Hashes

                if (UInt160.TryParse(token.Value, out var hash160)) return hash160;

                if (UInt256.TryParse(token.Value, out var hash256)) return hash256;
            }

            return token.Value;
        }

        static void AddTo(object obj, List<object> ret, Stack<List<object>> arrays)
        {
            if (arrays.Count > 0)
            {
                arrays.Peek().Add(obj);
            }
            else
            {
                ret.Add(obj);
            }
        }

        /// <summary>
        /// Here we don't know the type that we should return, and we need to create the best one
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return object</returns>
        static object ParseObjectFromString(string value)
        {
            // Separate Array tokens

            var tks = CleanCommand(value.SplitCommandLine());

            // Fetch parameters

            var ret = new List<object>();
            var arrays = new Stack<List<object>>();

            foreach (var token in tks)
            {
                var val = token.Value;

                if (token.Quoted)
                {
                    AddTo(ParseObjectFromToken(token), ret, arrays);
                }
                else
                {
                    switch (val)
                    {
                        case "[":
                            {
                                var ls = new List<object>();
                                arrays.Push(ls);
                                break;
                            }
                        case "]":
                            {
                                var ls = arrays.Pop();
                                AddTo(ls.ToArray(), ret, arrays);
                                break;
                            }
                        default:
                            {
                                AddTo(ParseObjectFromToken(token), ret, arrays);
                                break;
                            }
                    }
                }
            }

            if (arrays.Count > 0) throw new ArgumentException();

            return ret.Count == 1 ? ret[0] : ret.ToArray();
        }

        static private CommandToken[] CleanCommand(IEnumerable<CommandToken> tokens)
        {
            var change = false;
            var tks = new List<CommandToken>();

            foreach (var token in tokens)
            {
                if (token.Quoted || token.Value == "[" || token.Value == "]")
                {
                    tks.Add(token);
                }
                else
                {
                    var val = token.Value;
                    if (val.StartsWith("["))
                    {
                        tks.Add(new CommandToken("["));
                        val = val.Substring(1);
                        change = true;
                    }

                    CommandToken add = null;

                    if (val.EndsWith("]"))
                    {
                        add = new CommandToken("]");
                        val = val.Substring(0, val.Length - 1);
                        change = true;
                    }

                    if (!string.IsNullOrEmpty(val))
                        tks.Add(new CommandToken(val, false));

                    if (add != null)
                        tks.Add(add);
                }
            }

            // Recursive
            if (change) return CleanCommand(tks);

            return tks.ToArray();
        }

        /// <summary>
        /// Parse argument
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="type">Type</param>
        /// <returns>Return parsed argument</returns>
        private object ParseToArgument(CommandToken token, Type type)
        {
            // Custom converters

            if (_customConvertes.TryGetValue(type, out var converter))
            {
                return converter(token);
            }

            // Array
            if (type.IsArray)
            {
                var l = new List<object>();
                var gt = type.GetElementType();

                foreach (var ii in token.Value.Split(_splitChars))
                {
                    var ov = ParseToArgument(new CommandToken(ii, false), gt);
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
                foreach (var ii in token.Value.Split(_splitChars))
                {
                    var ov = ParseToArgument(new CommandToken(ii, false), gt);
                    if (ov == null) continue;

                    l.Add(ov);
                }
                return l;
            }

            if (type.IsEnum)
            {
                var iret = 0L;
                var f = Enum.GetNames(type);

                foreach (var en in token.Value.Split(new char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    foreach (var v in f)
                    {
                        if (string.Compare(v, en, true) == 0)
                        {
                            var v1 = Enum.Parse(type, v);
                            if (v1 != null)
                            {
                                if (iret == 0L)
                                {
                                    iret = Convert.ToInt64(v1);
                                }
                                else
                                {
                                    // Multienum One|Two
                                    iret |= Convert.ToInt64(v1);
                                }
                            }
                        }
                    }
                }

                return Enum.ToObject(type, iret);
            }

            // Is Convertible

            var conv = System.ComponentModel.TypeDescriptor.GetConverter(type);

            if (conv.CanConvertFrom(_stringType))
            {
                return conv.ConvertFrom(token.Value);
            }

            throw (new ArgumentException());
        }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return string.Join(",", Commands);
        }
    }
}