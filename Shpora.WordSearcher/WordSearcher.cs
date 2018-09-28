using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shpora.WordSearcher
{
    public class WordSearcher
    {
        private readonly WordSearcherGameClient wsClient;

        public WordSearcher(WordSearcherGameClient wsClient)
        {
            this.wsClient = wsClient;
        }

        public async Task PlaySessionAsync(bool useTestMap = false)
        {
            try
            {
                await wsClient.InitGameAsync(useTestMap);
                var words = await SearchForWords();
                foreach (var word in words.OrderBy(w => w.Length))
                {
                    await wsClient.SubmitWordsAsync(word);
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
            finally
            {
                if (wsClient.SessionInProgress)
                {
                    await wsClient.UpdateStatsAsync();
                    await wsClient.FinishGameAsync();
                    wsClient.State.LogStats();
                }
            }
        }

        private async Task<List<string>> SearchForWords()
        {
            var (width, height) = await new AdvancedMapDimensionsEstimator(wsClient).EstimateDimensions();
            Logger.Log.Info($"Estimated map size: {width}x{height}");
            var map = await new SimpleMapScanner(wsClient, width, height).ScanMapAsync();

            var letters = FindLetters(map);
            var words = TransformFoundLettersToWords(letters, width);

            if (words.Any())
                Logger.Log.Info("Found words: " + string.Join(", ", words));
            else
                Logger.Log.Warn("Found no words");

            return words;
        }

        private static List<(int x, int y, char c)> FindLetters(bool[,] map)
        {
            Logger.Log.Info("Looking for letters...");
            var lettersFound = 0;
            var width = map.GetLength(0);
            var height = map.GetLength(1);
            var letters = new List<(int x, int y, char c)>();
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                var fragmentHash = map.CustomFragmentHashCode(x, y, 7, 7);
                if (Letters.TryGetLetter(fragmentHash, out var letter))
                {
                    letters.Add((x, y, letter));
                    lettersFound++;
                }
            }

            Logger.Log.Info("Letters found: " + lettersFound);

            return letters;
        }

        private static List<string> TransformFoundLettersToWords(List<(int x, int y, char c)> letters, int width)
        {
            var coordsToLetters = letters.ToDictionary(l => (l.x, l.y), l => l.c);
            var words = new List<string>();
            while (coordsToLetters.Count > 0)
            {
                var wordBuilder = new StringBuilder();
                var (x, y) = coordsToLetters.Keys.First();
                while (coordsToLetters.ContainsKey(((x - Constants.LetterSize - 1 + width) % width, y)))
                    x = (x - Constants.LetterSize - 1 + width) % width;
                for (; coordsToLetters.ContainsKey((x, y)); x = (x + Constants.LetterSize + 1) % width)
                {
                    wordBuilder.Append(coordsToLetters[(x, y)]);
                    coordsToLetters.Remove((x, y));
                }

                words.Add(wordBuilder.ToString());
            }

            return words;
        }
    }
}
