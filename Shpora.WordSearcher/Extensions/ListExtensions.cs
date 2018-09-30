using System;
using System.Collections.Generic;
using System.Linq;

namespace Shpora.WordSearcher.Extensions
{
    public static class ListExtensions
    {
        public static int Median(this List<int> source)
        {
            var list = source.OrderBy(n => n).ToList();
            if (list.Count % 2 == 0)
                return (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;
            return list[list.Count / 2];
        }

        public static TS WithMax<TS, TKey>(this List<TS> source, Func<TS, TKey> selector) where TKey : IComparable<TKey>
        {
            var maxKey = default(TKey);
            var maxValue = default(TS);
            var firstPassed = false;

            foreach (var value in source)
            {
                var key = selector(value);
                if (!firstPassed || maxKey.CompareTo(key) < 0)
                {
                    maxKey = key;
                    maxValue = value;
                    firstPassed = true;
                }
            }

            return maxValue;
        }

        public static IEnumerable<T> Reversed<T>(this List<T> list)
        {
            for (var i = list.Count - 1; i >= 0; i--)
                yield return list[i];
        }
    }
}
