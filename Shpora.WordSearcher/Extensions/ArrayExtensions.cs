using System;

namespace Shpora.WordSearcher.Extensions
{
    public static class ArrayExtensions
    {
        public static bool Any<T>(this T[,] array, Func<T, bool> predicate)
        {
            for (var i = 0; i < array.GetLength(0); i++)
            for (var j = 0; j < array.GetLength(1); j++)
                if (predicate(array[i, j]))
                    return true;
            return false;
        }

        public static long CustomHashCode(this bool[,] array)
        {
            var hash = 0L;
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

        public static long CustomFragmentHashCode(
            this bool[,] array,
            int startX,
            int startY,
            int fragmentWidth,
            int fragmentHeight)
        {
            var hash = 0L;
            var width = array.GetLength(0);
            var height = array.GetLength(1);

            unchecked
            {
                for (var x = 0; x < fragmentWidth; x++)
                for (var y = 0; y < fragmentHeight; y++)
                {
                    var actualX = (startX + x) % width;
                    var actualY = (startY + y) % height;
                    hash = hash << 1;
                    if (array[actualX, actualY])
                        hash |= 1;
                }
            }

            return hash;
        }
    }
}
