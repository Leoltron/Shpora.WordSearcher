using System;
using System.Collections.Generic;

namespace Shpora.WordSearcher
{
    public static class ArrayExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T[,] array)
        {
            for (var i = 0; i < array.GetLength(0); i++)
            for (var j = 0; j < array.GetLength(1); j++)
                yield return array[i, j];
        }

        public static bool Any<T>(this T[,] array, Func<T, bool> predicate)
        {
            for (var i = 0; i < array.GetLength(0); i++)
            for (var j = 0; j < array.GetLength(1); j++)
                if (predicate(array[i, j]))
                    return true;
            return false;
        }

        public static ulong CustomHashCode(this bool[,] array)
        {
            ulong hash = 0L;
            unchecked
            {
                for (var i = 0; i < array.GetLength(0); i++)
                for (var j = 0; j < array.GetLength(1); j++)
                {
                    hash = hash << 1;
                    if (array[i, j])
                        hash |= 1;
                }
            }

            return hash;
        }

        public static bool ArrayEquals(this bool[,] array, bool[,] other)
        {
            if (array == null && other == null)
                return true;
            if (array == null || other == null)
                return false;
            if (array.GetLength(0) != other.GetLength(0) || array.GetLength(1) != other.GetLength(1))
                return false;

            for (var i = 0; i < array.GetLength(0); i++)
            for (var j = 0; j < array.GetLength(1); j++)
            {
                if (array[i, j] != other[i, j])
                    return false;
            }
            return true;
        }
    }
}
