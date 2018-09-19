using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shpora.WordSearcher
{
    public class MapDimensionsEstimator
    {
        private readonly WordSearcherGameClient wsGameClient;

        public MapDimensionsEstimator(WordSearcherGameClient wsGameClient)
        {
            this.wsGameClient = wsGameClient;
        }

        public async Task<(int width, int height)> EstimateDimensions()
        {
            return (await MakeCircle(Direction.Right), await MakeCircle(Direction.Down));
        }

        private async Task<int> MakeCircle(Direction direction)
        {
            await FindNonEmptyView(wsGameClient);
            var borderView = new bool[Constants.VisibleFieldWidth, Constants.VisibleFieldHeight];
            Array.Copy(wsGameClient.CurrentView, borderView, wsGameClient.CurrentView.Length);

            var viewHashes = new List<long>();
            var length = 0;
            while (true)
            {
                do
                {
                    await wsGameClient.Move(direction);
                    viewHashes.Add(wsGameClient.CurrentView.CustomHashCode());
                    length++;
                } while (!wsGameClient.CurrentView.ArrayEquals(borderView));

                var checkViewHashes = new List<long>(length);
                var patternMatches = true;
                for (var i = 0; i < length; i++)
                {
                    await wsGameClient.Move(direction);
                    checkViewHashes.Add(wsGameClient.CurrentView.CustomHashCode());
                    if (checkViewHashes[i] != viewHashes[i])
                    {
                        patternMatches = false;
                        viewHashes.AddRange(checkViewHashes);
                        break;
                    }
                }

                if (patternMatches)
                    break;
            }

            return length;
        }

        private static async Task FindNonEmptyView(WordSearcherGameClient wsGameClient)
        {
            Logger.Info("Looking for non-empty view for width and height estimation...");
            var linesChecked = 0;
            while (true)
            {
                var searchRange = 50 + linesChecked / 5 * 20;
                var foundSomething =
                    await MoveUntilSeeAnything(wsGameClient, (linesChecked & 1) == 0 ? Direction.Right : Direction.Left,
                        searchRange);
                if (foundSomething)
                    break;

                Logger.Info(
                    $"Found nothing in range {searchRange} on line, going {Constants.VisibleFieldHeight} lower... (current Y:{wsGameClient.Y})");
                linesChecked++;
                await wsGameClient.Move(Direction.Down, Constants.VisibleFieldHeight);
            }

            Logger.Info("Non-empty view found.");
        }

        private static async Task<bool> MoveUntilSeeAnything(WordSearcherGameClient wsGameClient, Direction direction,
            int maxMoves = -1)
        {
            while (!wsGameClient.SeesAnything && maxMoves != 0)
            {
                await wsGameClient.Move(direction);
                if (maxMoves > 0)
                    maxMoves--;
            }

            return wsGameClient.SeesAnything;
        }
    }
}
