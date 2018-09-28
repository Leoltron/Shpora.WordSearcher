using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shpora.WordSearcher
{
    public class OtherMapDimensionsEstimator : MapDimensionsEstimator
    {
        private readonly LetterSearcher letterSearcher;

        public OtherMapDimensionsEstimator(WordSearcherGameClient wsGameClient) : base(wsGameClient)
        {
            letterSearcher = new LetterSearcher(wsGameClient);
        }

        public override async Task<(int width, int height)> EstimateDimensions()
        {
            var (letterX, letterY) = await MoveToAnyLetter();
            letterY = await MoveToSeeLetterTop(letterY);
            Logger.Log.Info("Moving to word left border...");
            var (lastLetterX, lastLetterY) = await MoveUntilWordEnd(letterX, letterY, Direction.Left);
            wsGameClient.ViewHashRecorder.StartNewRecording();

            Logger.Log.Info("Moving to word right border...");
            await MoveUntilWordEnd(lastLetterX, lastLetterY, Direction.Right);
            var keyWordHashes = wsGameClient.ViewHashRecorder.LastViewHashes;

            Logger.Log.Info("Now I have \"word hash\". Starting estimation...");

            wsGameClient.ViewHashRecorder.ClearPreviousHashes();
            return await EstimateDimensionsByKeyword(keyWordHashes);
        }

        private async Task<int> MoveToSeeLetterTop(int letterY)
        {
            if (letterY != 0)
                await wsGameClient.MoveAsync(0, letterY);
            return 0;

        }

        private async Task<(int width, int height)> EstimateDimensionsByKeyword(List<long> keyWordHashes)
        {
            var width = await EstimateWidthByKeyword(keyWordHashes);
            var height = await EstimateHeightByKeyword(keyWordHashes);
            return (width, height);
        }

        private async Task<int> EstimateHeightByKeyword(List<long> keyWordHashes)
        {
            var lastHash = keyWordHashes.Last();
            Logger.Log.Info("Estimating height by moving down until word last hash repeats");
            var startY = wsGameClient.State.Y;
            while (true)
            {
                await MoveUntilViewHash(lastHash, Direction.Down);

                Logger.Log.Info("Found word last hash, checking word hash...");
                int i;
                for (i = 1; i < keyWordHashes.Count; i++)
                {
                    await wsGameClient.MoveAsync(Direction.Left);
                    if (wsGameClient.State.CurrentView.CustomHashCode() != keyWordHashes[keyWordHashes.Count - 1 - i])
                        break;
                }

                if (i < keyWordHashes.Count)
                {
                    Logger.Log.Info("Wrong word, back to searching...");
                    await wsGameClient.MoveAsync(Direction.Right, i);
                }
                else
                {
                    break;
                }
            }

            var height = wsGameClient.State.Y - startY;
            Logger.Log.Info("Success, height: " + height);
            return height;
        }

        private async Task<int> EstimateWidthByKeyword(List<long> keyWordHashes)
        {
            Logger.Log.Info("Estimating width by moving right until word hash repeats");
            var startX = wsGameClient.State.X;
            while (!wsGameClient.ViewHashRecorder.IsHashesEndsWith(keyWordHashes))
            {
                await wsGameClient.MoveAsync(Direction.Right);
            }

            var width = wsGameClient.State.X - startX;
            Logger.Log.Info($"Success, width: {width}");
            wsGameClient.ViewHashRecorder.StopRecording();
            return width;
        }

        private async Task MoveUntilViewHash(long lastHash, Direction direction)
        {
            do
            {
                await wsGameClient.MoveAsync(direction);
            } while (wsGameClient.State.CurrentView.CustomHashCode() != lastHash);
        }

        private async Task<(int letterX, int letterY)> MoveToAnyLetter()
        {
            await FindNonEmptyView(wsGameClient);
            Logger.Log.Info("Moving until see any letter clearly");
            var maxArea = Math.Min(Constants.VisibleFieldWidth, Constants.LetterSize) *
                          Math.Min(Constants.VisibleFieldHeight, Constants.LetterSize);

            var letters = letterSearcher.FindLettersInCurrentView(true).Where(l => l.c != 'г' && l.c != 'т').ToList();
            while (letters.Any() && letters.Max(l => l.visibleArea) < maxArea)
            {
                var avgX = letters.Sum(l => l.relativeX) / letters.Count;
                var avgY = letters.Sum(l => l.relativeY) / letters.Count;
                if (avgY == 0 && avgX == 0)
                {
                    avgX = letters.Select(l => l.relativeX).ToList().Median();
                    avgY = letters.Select(l => l.relativeY).ToList().Median();
                }

                Logger.Log.Info($"Moving by ({avgX}, {avgY})");
                await wsGameClient.MoveAsync(avgX, avgY);

                letters = letterSearcher.FindLettersInCurrentView(true);
            }

            var (letterX, letterY, _, _) = letters.WithMax(l => l.visibleArea);
            return (letterX, letterY);
        }

        private async Task<(int lastLetterX, int lastLetterY)> MoveUntilWordEnd(int letterViewX, int letterViewY,
            Direction direction)
        {
            if (direction != Direction.Left && direction != Direction.Right)
                throw new ArgumentException("Expected direction to be Left or Right, got: " + direction,
                    nameof(direction));

            var lastLetterX = letterViewX;
            var lastLetterY = letterViewY;

            var dirDx = direction == Direction.Right ? 1 : -1;
            var nextLetterRelativeX = dirDx * (Constants.LetterSize + 1);
            var nextLetterX = letterViewX + nextLetterRelativeX;

            Func<bool> canTryScanNextLetter;
            if (direction == Direction.Right)
                canTryScanNextLetter = () => nextLetterX < Constants.VisibleFieldWidth - Constants.LetterSize;
            else
                canTryScanNextLetter = () => nextLetterX >= 0;

            while (true)
            {
                await wsGameClient.MoveAsync(direction);
                nextLetterX -= dirDx;
                lastLetterX -= dirDx;
                Logger.Log.Warn($"Looking at {nextLetterX}, {letterViewY}");
                if (!canTryScanNextLetter())
                    continue;
                var letters = letterSearcher.FindLettersInCurrentView(true);
                if (letters.Any(l => l.relativeX == nextLetterX && l.relativeY == letterViewY))
                {
                    lastLetterX = nextLetterX;
                    nextLetterX += nextLetterRelativeX;
                }
                else
                {
                    return (lastLetterX, lastLetterY);
                }
            }
        }
    }
}
