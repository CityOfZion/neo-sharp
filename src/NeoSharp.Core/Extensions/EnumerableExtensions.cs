using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace NeoSharp.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public enum QueryMode
        {
            Regex,
            Contains
        }

        /// <summary>
        /// Query result
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="enumerable">Enumerable</param>
        /// <param name="query">Query</param>
        /// <param name="mode">Mode</param>
        /// <returns>Filtered results</returns>
        public static IEnumerable<T> QueryResult<T>(this IEnumerable<T> enumerable, string query, QueryMode mode)
        {
            switch (mode)
            {
                case QueryMode.Contains:
                    {
                        foreach (var obj in enumerable)
                        {
                            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);

                            if (json.Contains(query))
                            {
                                yield return obj;
                            }
                        }
                        break;
                    }
                case QueryMode.Regex:
                    {
                        var regex = new Regex(query);

                        foreach (var obj in enumerable)
                        {
                            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);

                            if (regex.IsMatch(json))
                            {
                                yield return obj;
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Performs the action for each element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Source</param>
        /// <param name="action">Action</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static IEnumerable<TSource> Distinct<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var keys = new HashSet<TKey>();

            foreach (var item in source)
            {
                if (keys.Add(keySelector(item)))
                {
                    yield return item;
                }
            }
        }
    }
}