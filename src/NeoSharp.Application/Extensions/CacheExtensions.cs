using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NeoSharp.Application.Attributes;
using NeoSharp.Application.Client;
using NeoSharp.Core.DI;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;

namespace NeoSharp.Application.Extensions
{
    public static class CacheExtensions
    {
        public static void Cache(this IDictionary<string[], PromptCommandAttribute> cache, object instance, IAutoCompleteHandler autoComplete)
        {
            foreach (var mi in instance.GetType().GetMethods
                (
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
                ))
            {
                foreach (var atr in mi.GetCustomAttributes<PromptCommandAttribute>())
                {
                    if (atr == null) continue;

                    atr.SetMethod(instance, mi);
                    cache.Add(atr.Commands, atr);

                    if (autoComplete == null) continue;

                    if (autoComplete.TryGetMethods(atr.Command, out var value))
                    {
                        value.Add(mi.GetParameters());
                    }
                    else
                    {
                        var ls = new List<ParameterInfo[]>
                    {
                        mi.GetParameters()
                    };

                        autoComplete.Add(atr.Command, ls);
                    }
                }
            }
        }

        public static T SearchRightCommand<T>(this T[] cmds, IEnumerable<CommandToken> tokens, IContainer injector, out object[] args) where T : PromptCommandAttribute
        {
            args = null;
            var cmd = default(T);

            foreach (var a in cmds)
            {
                try
                {
                    var ret = a.ConvertToArguments(tokens.Skip(a.TokensCount).ToArray(), injector);

                    if (cmd == null || cmd.Order > a.Order)
                    {
                        cmd = a;
                        args = ret;
                    }
                }
                catch { }
            }

            return cmd;
        }

        public static IEnumerable<T> SearchCommands<T>(this IDictionary<string[], T> cache, IList<CommandToken> cmdArgs)
        {
            // Parse arguments

            if (cmdArgs.Count <= 0) yield break;

            foreach (var key in cache)
            {
                if (key.Key.Length > cmdArgs.Count) continue;

                var equal = true;
                for (int x = 0, m = key.Key.Length; x < m; x++)
                {
                    var c = cmdArgs[x];
                    if (c.Value.ToLowerInvariant() != key.Key[x])
                    {
                        equal = false;
                        break;
                    }
                }

                if (equal)
                {
                    yield return key.Value;
                }
            }
        }
    }
}