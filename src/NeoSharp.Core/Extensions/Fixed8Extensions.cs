using NeoSharp.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoSharp.Core.Extensions
{
    public static class Fixed8Extensions
    {
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

        public static Fixed8 Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Fixed8> selector)
        {
            return source.Select(selector).Sum();
        }

        public static Fixed8 Max(this IEnumerable<Fixed8> source)
        {
            var currentFixed8 = new Fixed8();

            foreach (var other in source)
            {
                if (currentFixed8.CompareTo(other) < 0)
                {
                    currentFixed8 = other;
                }
            }
            return currentFixed8;
        }

        public static Fixed8 Min(this IEnumerable<Fixed8> source)
        {
            var currentFixed8 = new Fixed8();

            foreach (var other in source)
            {
                if (currentFixed8.CompareTo(other) > 0)
                {
                    currentFixed8 = other;
                }
            }
            return currentFixed8;
        }
    }
}
