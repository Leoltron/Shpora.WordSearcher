using System.Linq;

namespace Shpora.WordSearcher
{
    public  static class ViewHelper
    {
        public static bool[,] LayoutToView(string[] layout)
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
