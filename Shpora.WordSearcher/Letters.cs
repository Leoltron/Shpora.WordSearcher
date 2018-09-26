using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Shpora.WordSearcher
{
    public static class Letters
    {
        private static readonly Dictionary<long, char> ViewHashToChar = Directory.GetFiles("./Letters")
            .ToDictionary(
                fn => LayoutToView(File.ReadAllLines(fn)).CustomHashCode(),
                fn => Path.GetFileName(fn).ToLowerInvariant()[0]);

        public static bool TryGetLetter(long viewHash, out char letter)
        {
            return ViewHashToChar.TryGetValue(viewHash, out letter);
        }

        private static bool[,] LayoutToView(string[] layout)
        {
            var view = new bool[layout.Max(s => s.Length), layout.Length];
            for (var y = 0; y < layout.Length; y++)
            for (var x = 0; x < layout[y].Length; x++)
            {
                view[x, y] = layout[y][x] == '#';
            }

            return view;
        }
    }
}
