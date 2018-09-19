using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static Shpora.WordSearcher.Direction;

namespace Shpora.WordSearcher
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var (server, token) = ParseArgs(args);
            var client = new HttpClient {BaseAddress = new Uri(server)};
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);

            var ws = new WordSearcher(client);
            await ws.InitGameAsync(false);
            try
            {
                await WsMain(ws);
            }
            finally
            {
                await ws.FinishGameAsync();
                Console.WriteLine("Session finished. Total points: " + ws.Points);
            }
        }

        private static async Task WsMain(WordSearcher ws)
        {
            await FindNonEmptyView(ws);

            Console.WriteLine("Estimating width...");
            Console.WriteLine("Going right until find border view again...");
            var borderView = new bool[Constants.VisibleFieldWidth, Constants.VisibleFieldHeight];
            Array.Copy(ws.CurrentView, borderView, ws.CurrentView.Length);

            var viewHashes = new List<long>();
            var width = 0;
            while (true)
            {
                do
                {
                    await ws.Move(Right);
                    viewHashes.Add(ws.CurrentView.CustomHashCode());
                    width++;
                } while (!ws.CurrentView.ArrayEquals(borderView));

                Console.WriteLine(
                    "Border view found again, going right further to check that line is repeating itself...");

                var checkViewHashes = new List<long>(width);
                var patternMatches = true;
                for (var i = 0; i < width; i++)
                {
                    await ws.Move(Right);
                    checkViewHashes.Add(ws.CurrentView.CustomHashCode());
                    if (checkViewHashes[i] != viewHashes[i])
                    {
                        patternMatches = false;
                        viewHashes.AddRange(checkViewHashes);
                        Console.WriteLine("Got an unexpected view, continuing searching for border view..");
                        break;
                    }
                }

                if (patternMatches)
                    break;
            }

            Console.WriteLine("Line repeated, estimated width most likely is correct.");
            Console.WriteLine("Estimated width: " + width);

            Console.WriteLine("Estimating height...");
            Console.WriteLine("Going down until find border view again...");
            viewHashes.Clear();
            var height = 0;
            while (true)
            {
                do
                {
                    await ws.Move(Down);
                    viewHashes.Add(ws.CurrentView.CustomHashCode());
                    height++;
                } while (!ws.CurrentView.ArrayEquals(borderView));

                Console.WriteLine(
                    "Border view found again, going down further to check that column is repeating itself...");

                var checkViewHashes = new List<long>(height);
                var patternMatches = true;
                for (var i = 0; i < height; i++)
                {
                    await ws.Move(Down);
                    checkViewHashes.Add(ws.CurrentView.CustomHashCode());
                    if (checkViewHashes[i] != viewHashes[i])
                    {
                        patternMatches = false;
                        viewHashes.AddRange(checkViewHashes);
                        Console.WriteLine("Got an unexpected view, continuing searching for border view..");
                        break;
                    }
                }

                if (patternMatches)
                    break;
            }

            Console.WriteLine("Column repeated, estimated height most likely is correct.");
            Console.WriteLine("Estimated height: " + height);

            Console.WriteLine("Scanning map...");
            var map = await ScanMap(ws, width, height);
            var letters = FindLetters(map);
            var coordsToLetters = letters.ToDictionary(l => (l.x, l.y), l => l.c);
            var words = new List<string>();
            while (coordsToLetters.Count > 0)
            {
                var wordBuilder = new StringBuilder();
                var (x, y) = coordsToLetters.Keys.First();
                while (coordsToLetters.ContainsKey((((x - 7 - 1) + width) % width, y)))
                    x = (x - 7 - 1 + width) % width;
                for (; coordsToLetters.ContainsKey((x, y)); x = (x + 7 + 1) % width)
                {
                    wordBuilder.Append(coordsToLetters[(x, y)]);
                    coordsToLetters.Remove((x, y));
                }

                Console.WriteLine("Found word " + wordBuilder);
                words.Add(wordBuilder.ToString());
            }

            await ws.UpdateStatsAsync();
            var pointsLost = -ws.Points;

            foreach (var word in words.OrderBy(w => w.Length))
            {
                await ws.SubmitWords(word);
            }
            await ws.UpdateStatsAsync();

            Console.WriteLine("Moves: " + ws.Moves);
            Console.WriteLine("Points lost by walking: " + pointsLost);
            Console.WriteLine("Total words: " + ws.Words);
            Console.WriteLine("Got from word submitting: " + (ws.Points + pointsLost));
        }

        private static List<(int x, int y, char c)> FindLetters(bool[,] map)
        {
            Console.WriteLine("Looking for letters...");
            var lettersFound = 0;
            var width = map.GetLength(0);
            var height = map.GetLength(1);
            var letters = new List<(int x, int y, char c)>();
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                var fragmentHash = map.CustomFragmentHashCode(x, y, 7, 7);
                if (Letters.AlphabetViews.TryGetValue(fragmentHash, out var letter))
                {
                    letters.Add((x, y, letter));
                    lettersFound++;
                    Console.Write("\rLetters found: " + lettersFound);
                }
            }

            if (lettersFound != 0)
                Console.WriteLine();

            return letters;
        }

        private static async Task<bool[,]> ScanMap(WordSearcher ws, int width, int height)
        {
            var map = new bool[width, height];
            void UpdateMap() => CopyMap(ws.CurrentView, map, ws.X, ws.Y);
            ws.ResetCoords();
            var linesRemain = height;
            UpdateMap();
            Console.Write("Scan progress: 0%");
            while (true)
            {
                for (var i = 0; i < width - Constants.VisibleFieldWidth; i++)
                {
                    await ws.Move(Right);
                    if (i % Constants.VisibleFieldWidth == 0)
                        UpdateMap();
                }

                UpdateMap();
                linesRemain -= Constants.VisibleFieldHeight;
                if (linesRemain <= 0) break;
                await ws.Move(Down, Math.Min(linesRemain, Constants.VisibleFieldHeight));
                UpdateMap();
                Console.Write($"\rScan progress: {(int) (100 - (float) linesRemain / height * 100)}%");
            }

            Console.WriteLine("\rScan progress: 100%");

            return map;
        }

        private static void CopyMap(bool[,] from, bool[,] to, int offsetX, int offsetY)
        {
            var fromWidth = from.GetLength(0);
            var fromHeight = from.GetLength(1);
            var toWidth = to.GetLength(0);
            var toHeight = to.GetLength(1);
            for (var x = 0; x < fromWidth; x++)
            for (var y = 0; y < fromHeight; y++)
            {
                var toX = (x + offsetX) % toWidth;
                var toY = (y + offsetY) % toHeight;
                to[toX, toY] = from[x, y];
            }
        }

        private static async Task FindNonEmptyView(WordSearcher ws)
        {
            Console.WriteLine("Looking for non-empty view for width and height estimation...");
            var linesChecked = 0;
            while (true)
            {
                var searchRange = 50 + linesChecked / 5 * 20;
                var foundSomething =
                    await MoveUntilSeeAnything(ws, (linesChecked & 1) == 0 ? Right : Left, searchRange);
                if (foundSomething)
                    break;

                Console.WriteLine(
                    $"Found nothing in range {searchRange} on line, going {Constants.VisibleFieldHeight} lower... (current Y:{ws.Y})");
                linesChecked++;
                await ws.Move(Down, Constants.VisibleFieldHeight);
            }

            Console.WriteLine("Non-empty view found.");
        }

        private static async Task<bool> MoveUntilSeeAnything(WordSearcher ws, Direction direction, int maxMoves = -1)
        {
            while (!ws.SeesAnything && maxMoves != 0)
            {
                await ws.Move(direction);
                if (maxMoves > 0)
                    maxMoves--;
            }

            return ws.SeesAnything;
        }

        private static (string server, string token) ParseArgs(string[] args)
        {
            if (args == null || args.Length != 2)
            {
                return ("http://shpora.skbkontur.ru:80/", "4b127b29-d38d-4c72-8276-e79e04c3095b");
            }

            return (args[0], args[1]);
        }
    }
}