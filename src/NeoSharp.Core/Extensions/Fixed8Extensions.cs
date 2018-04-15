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
    }
}
