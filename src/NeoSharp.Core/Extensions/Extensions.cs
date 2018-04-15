using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NeoSharp.Core.Types;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NeoSharp.Core.Extensions
{
    public static class Extensions
    {
        internal static IEnumerable<TResult> WeightedFilter<T, TResult>(this IList<T> source, double start, double end, Func<T, long> weightSelector, Func<T, long, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (start < 0 || start > 1) throw new ArgumentOutOfRangeException(nameof(start));
            if (end < start || start + end > 1) throw new ArgumentOutOfRangeException(nameof(end));
            if (weightSelector == null) throw new ArgumentNullException(nameof(weightSelector));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            // TODO: check precision
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (source.Count == 0 || start == end) yield break;
            double amount = source.Sum(weightSelector);
            long sum = 0;
            double current = 0;
            foreach (var item in source)
            {
                if (current >= end) break;
                var weight = weightSelector(item);
                sum += weight;
                var old = current;
                current = sum / amount;
                if (current <= start) continue;
                if (old < start)
                {
                    if (current > end)
                    {
                        weight = (long)((end - start) * amount);
                    }
                    else
                    {
                        weight = (long)((current - start) * amount);
                    }
                }
                else if (current > end)
                {
                    weight = (long)((end - old) * amount);
                }
                yield return resultSelector(item, weight);
            }
        }

        internal static int GetVarSize(int value)
        {
            if (value < 0xFD)
                return sizeof(byte);
            else if (value <= 0xFFFF)
                return sizeof(byte) + sizeof(ushort);
            else
                return sizeof(byte) + sizeof(uint);
        }

        internal static int GetVarSize<T>(this T[] value)
        {
            int valueSize;
            var t = typeof(T);
            if (typeof(ISerializable).IsAssignableFrom(t))
            {
                valueSize = value.OfType<ISerializable>().Sum(p => p.Size);
            }
            else if (t.GetTypeInfo().IsEnum)
            {
                int elementSize;
                var u = t.GetTypeInfo().GetEnumUnderlyingType();
                if (u == typeof(sbyte) || u == typeof(byte))
                    elementSize = 1;
                else if (u == typeof(short) || u == typeof(ushort))
                    elementSize = 2;
                else if (u == typeof(int) || u == typeof(uint))
                    elementSize = 4;
                else //if (u == typeof(long) || u == typeof(ulong))
                    elementSize = 8;
                valueSize = value.Length * elementSize;
            }
            else
            {
                valueSize = value.Length * Marshal.SizeOf<T>();
            }
            return GetVarSize(value.Length) + valueSize;
        }

        internal static int GetVarSize(this string value)
        {
            var size = Encoding.UTF8.GetByteCount(value);
            return GetVarSize(size) + size;
        }
    }
}
