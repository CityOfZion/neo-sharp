using System;
using System.Collections.Generic;
using System.Linq;
using NeoSharp.Types;

namespace NeoSharp.Core.Extensions
{
    public static class Fixed8Extensions
    {
        /// <summary>
        /// Fixed8 Sum
        /// </summary>
        /// <param name="source">Source</param>
        /// <returns>Sum Result</returns>
        public static Fixed8 Sum(this IEnumerable<Fixed8> source)
        {
            long sum = 0;

            checked
            {
                foreach (var item in source)
                {
                    sum += item.Value;
                }
            }

            return new Fixed8(sum);
        }

        /// <summary>
        /// Fixed8 Sum
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="selector">Selector</param>
        /// <returns>Sum Result</returns>
        public static Fixed8 Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Fixed8> selector)
        {
            return source.Select(selector).Sum();
        }

        /// <summary>
        /// Returns the higher value 
        /// </summary>
        /// <param name="source">Source</param>
        /// <returns>Max value</returns>
        public static Fixed8 Max(this IEnumerable<Fixed8> source)
        {
            var first = true;
            var currentFixed8 = new Fixed8();

            foreach (var other in source)
            {
                if (first || currentFixed8.CompareTo(other) < 0)
                {
                    currentFixed8 = other;
                    first = false;
                }
            }
            return currentFixed8;
        }

        /// <summary>
        /// Returns the lower value 
        /// </summary>
        /// <param name="source">Source</param>
        /// <returns>Min value</returns>
        public static Fixed8 Min(this IEnumerable<Fixed8> source)
        {
            var first = true;
            var currentFixed8 = new Fixed8();

            foreach (var other in source)
            {
                if (first || currentFixed8.CompareTo(other) > 0)
                {
                    currentFixed8 = other;
                    first = false;
                }
            }

            return currentFixed8;
        }
    }
}