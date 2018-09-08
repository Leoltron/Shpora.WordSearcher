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
    }
}