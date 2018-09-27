using System.Collections.Generic;
using System.Linq;

namespace Shpora.WordSearcher
{
    public static class ListExtensions
    {
        public static int Median(this IEnumerable<int> source)
        {
            var list = source.OrderBy(n => n).ToList();
            if (list.Count % 2 == 0)
                return (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;
            return list[list.Count / 2];
        }
    }
}
