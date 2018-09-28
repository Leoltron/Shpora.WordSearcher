using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static string ToViewString(this bool[,] view)
        {
            var lines = new List<string>();
            for (var y = 0; y < view.GetLength(1); y++)
            {
                var sb = new StringBuilder();

                for (var x = 0; x < view.GetLength(0); x++)
                    sb.Append(view[x, y] ? '#' : '_');
                lines.Add(sb.ToString());
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
