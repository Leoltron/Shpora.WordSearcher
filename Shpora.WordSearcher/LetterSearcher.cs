using System;
using System.Collections.Generic;
using Shpora.WordSearcher.Extensions;

namespace Shpora.WordSearcher
{
    public class LetterSearcher
    {
        private readonly WordSearcherGameClient wsClient;

        public LetterSearcher(WordSearcherGameClient wsClient)
        {
            this.wsClient = wsClient;
        }

        public List<(int relativeX, int relativeY, char c, int visibleArea)> FindLettersInCurrentView(bool ignoreEmptyFragments)
        {
            var letters = new List<(int, int, char, int)>();
            var viewRect = new Rect(0, 0, Constants.VisibleFieldWidth, Constants.VisibleFieldHeight);
            for (var x = -Constants.LetterSize + 1; x < Constants.VisibleFieldWidth; x++)
            for (var y = -Constants.LetterSize + 1; y < Constants.VisibleFieldHeight; y++)
            {
                var letterRect = new Rect(x, y, Constants.LetterSize, Constants.LetterSize);
                var inter = Intersection(viewRect, letterRect);

                if (!IsViewFragmentBordered(inter))
                    continue;
                var letterX = inter.X - x;
                var letterY = inter.Y - y;
                var fragmentHash = wsClient.State.CurrentView
                    .CustomFragmentHashCode(inter.X, inter.Y, inter.Width, inter.Height);
                if (ignoreEmptyFragments && fragmentHash == 0)
                    continue;
                foreach (var (c, view) in Letters.All)
                {
                    if (fragmentHash == view.CustomFragmentHashCode(letterX, letterY, inter.Width, inter.Height))
                    {
                        letters.Add((x, y, c, inter.Area));
                    }
                }
            }

            return letters;
        }

        private bool IsViewFragmentBordered(Rect fragment)
        {
            var borderLeft = Math.Max(0, fragment.Left - 1);
            var borderRight = Math.Min(Constants.VisibleFieldWidth - 1, fragment.Right + 1);
            var borderTop = Math.Max(0, fragment.Top - 1);
            var borderBottom = Math.Min(Constants.VisibleFieldHeight - 1, fragment.Bottom + 1);

            var view = wsClient.State.CurrentView;

            if (fragment.Left > 0 && fragment.Left < Constants.VisibleFieldWidth - 1)
                for (var y = borderTop; y <= borderBottom; y++)
                {
                    if (view[borderLeft, y])
                        return false;
                }

            if (fragment.Right > 0 && fragment.Right < Constants.VisibleFieldWidth - 1)
                for (var y = borderTop; y <= borderBottom; y++)
                {
                    if (view[borderRight, y])
                        return false;
                }

            if (fragment.Top > 0 && fragment.Top < Constants.VisibleFieldHeight - 1)
                for (var x = borderLeft; x <= borderRight; x++)
                {
                    if (view[x, borderTop])
                        return false;
                }

            if (fragment.Bottom > 0 && fragment.Bottom < Constants.VisibleFieldHeight - 1)
                for (var x = borderLeft; x <= borderRight; x++)
                {
                    if (view[x, borderBottom])
                        return false;
                }

            return true;
        }

        private static Rect Intersection(Rect r1, Rect r2)
        {
            var iRight = Math.Min(r1.Right, r2.Right);
            var iLeft = Math.Max(r1.Left, r2.Left);
            var iTop = Math.Max(r1.Top, r2.Top);
            var iBottom = Math.Min(r1.Bottom, r2.Bottom);

            return Rect.FromCoords(iLeft, iRight, iTop, iBottom);
        }

        private struct Rect
        {
            public readonly int X;
            public readonly int Y;
            public readonly int Width;
            public readonly int Height;

            public int Top => Y;
            public int Left => X;
            public int Right => X + Width - 1;
            public int Bottom => Y + Height - 1;
            public int Area => Width * Height;

            public Rect(int x, int y, int width, int height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            public static Rect FromCoords(int left, int right, int top, int bottom)
            {
                return new Rect(left, top, right - left + 1, bottom - top + 1);
            }
        }
    }
}
