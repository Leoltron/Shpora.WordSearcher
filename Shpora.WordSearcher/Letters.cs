using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shpora.WordSearcher.Extensions;

namespace Shpora.WordSearcher
{
    public static class Letters
    {
        public static readonly List<(char c, bool[,] view)> All = Directory.GetFiles("./Letters")
            .Select(fn => (Path.GetFileName(fn).ToLowerInvariant()[0], ViewHelper.LayoutToView(File.ReadAllLines(fn))))
            .ToList();

        private static readonly Dictionary<long, char> ViewHashToChar = All
            .ToDictionary(p => p.view.CustomHashCode(), p => p.c);

        public static bool TryGetLetter(long viewHash, out char letter)
        {
            return ViewHashToChar.TryGetValue(viewHash, out letter);
        }
    }
}
