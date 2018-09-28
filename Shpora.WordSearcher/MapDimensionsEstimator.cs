using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shpora.WordSearcher
{
    public class MapDimensionsEstimator
    {
        protected readonly WordSearcherGameClient wsGameClient;

        public MapDimensionsEstimator(WordSearcherGameClient wsGameClient)
        {
            this.wsGameClient = wsGameClient;
        }

        public virtual async Task<(int width, int height)> EstimateDimensions()
        {
            return (await MakeCircle(Direction.Right), await MakeCircle(Direction.Down));
        }

        /// <summary>
        /// Makes a circle around the map in attempt to estimate its width or height. Returns length of a circle.
        /// </summary>
        private async Task<int> MakeCircle(Direction direction, int minLength = Constants.LetterSize)
        {
            await FindNonEmptyView(wsGameClient);
            var borderView = new bool[Constants.VisibleFieldWidth, Constants.VisibleFieldHeight];
            Array.Copy(wsGameClient.State.CurrentView, borderView, wsGameClient.State.CurrentView.Length);

            var directionStr = direction.ToString().ToLowerInvariant();
            Logger.Log.Info($"Moving {directionStr} until see that non-empty view again");
            var viewHashes = new List<long>();
            var length = 0;
            while (true)
            {
                bool viewFound;
                do
                {
                    await wsGameClient.MoveAsync(direction);
                    viewHashes.Add(wsGameClient.State.CurrentView.CustomHashCode());
                    length++;
                    viewFound = wsGameClient.State.CurrentView.ArrayEquals(borderView);
                    if (viewFound && length < minLength)
                        Logger.Log.Warn("View found, but length is too little " +
                                    $"({length}, expected at least {minLength}), continuing search...");
                } while (!viewFound || length < minLength);

                Logger.Log.Info($"View found, going further {directionStr} to double-check that it's a full circle");
                var checkViewHashes = new List<long>(length);
                var patternMatches = true;
                for (var i = 0; i < length; i++)
                {
                    await wsGameClient.MoveAsync(direction);
                    checkViewHashes.Add(wsGameClient.State.CurrentView.CustomHashCode());
                    if (checkViewHashes[i] != viewHashes[i])
                    {
                        Logger.Log.Info("Incorrect view, back to searching...");
                        patternMatches = false;
                        viewHashes.AddRange(checkViewHashes);
                        break;
                    }
                }

                if (patternMatches)
                    break;
            }

            Logger.Log.Info($"Done, circle length is {length}.");

            return length;
        }

        protected static async Task FindNonEmptyView(WordSearcherGameClient wsGameClient)
        {
            if (wsGameClient.State.SeesAnything)
            {
                Logger.Log.Info("Already have a non-empty view, no search is required.");
                return;
            }

            Logger.Log.Info("Looking for a non-empty view for width and height estimation...");
            var linesChecked = 0;
            while (true)
            {
                var searchRange = 50 + linesChecked / 5 * 20;
                var foundSomething =
                    await MoveUntilSeeAnything(wsGameClient, linesChecked % 2 == 0 ? Direction.Right : Direction.Left,
                        searchRange);
                if (foundSomething)
                    break;

                Logger.Log.Info(
                    $"Found nothing in range {searchRange} on line, going {Constants.VisibleFieldHeight} lower... (current Y:{wsGameClient.State.Y})");
                linesChecked++;
                await wsGameClient.MoveAsync(Direction.Down, Constants.VisibleFieldHeight);
            }
            /*
            var ls = new LetterSearcher(wsGameClient);
            var ltrs = ls.FindLettersInCurrentView();
            while (ltrs.Any() && ltrs.Max(l => l.visibleArea) * 100 / (Constants.LetterSize * Constants.LetterSize) < 60)
            {
                var x = ltrs.Sum(l => l.relativeX) / ltrs.Count;
                var y = ltrs.Sum(l => l.relativeY) / ltrs.Count;
                Logger.Log.Info($"Moving by ({x}, {y})");
                if (x != 0)
                    await wsGameClient.MoveAsync(x > 0 ? Direction.Right : Direction.Left, Math.Abs(x));
                if (y != 0)
                    await wsGameClient.MoveAsync(y > 0 ? Direction.Down : Direction.Up, Math.Abs(y));
                ltrs = ls.FindLettersInCurrentView();
            }
            */

            Logger.Log.Info("Non-empty view found.");
        }

        private static async Task<bool> MoveUntilSeeAnything(WordSearcherGameClient wsGameClient, Direction direction,
            int maxMoves = -1)
        {
            while (!wsGameClient.State.SeesAnything && maxMoves != 0)
            {
                await wsGameClient.MoveAsync(direction);
                if (maxMoves > 0)
                    maxMoves--;
            }

            return wsGameClient.State.SeesAnything;
        }
    }
}
