using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shpora.WordSearcher
{
    public class WordSearcher
    {
        private readonly HttpClient client;
        private static readonly bool[,] emptyField = new bool[0, 0];
        public int Points { get; private set; }
        public int Words { get; private set; }
        public int Moves { get; private set; }
        public bool[,] CurrentView { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public bool SeesAnything => CurrentView?.Any(b => b) ?? false;

        public WordSearcher(HttpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task InitGameAsync(bool test = false, bool retryIfConflict = false)
        {
            var request = "/task/game/start";
            if (test)
                request += "?test=true";

            var response = await client.PostAsync(request, null);
            response.EnsureSuccessStatusCode();
            //var secondsTimeout = long.Parse(response.Headers.GetValues("Expires").First());
            //var sessionInitDate = DateTime.Parse(string.Join(" ", response.Headers.GetValues("Last-Modified")));
            ResetStats();
        }

        private void ResetStats()
        {
            Points = 0;
            ResetCoords();
        }

        public void ResetCoords()
        {
            X = 0;
            Y = 0;
        }

        public async Task UpdateStatsAsync()
        {
            var response = await client.GetAsync("/task/game/stats");
            var stats = await response.DeserializeContent<Dictionary<string, int>>();
            Words = stats["words"];
            Points = stats["points"];
            Moves = stats["moves"];
        }

        public async Task Move(Direction direction, int amount, bool updateView = true)
        {
            //Console.WriteLine($"Before move X:{X} Y:{Y}");
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Value must be positive");
            var request = $"/task/move/{direction}";
            HttpResponseMessage response = null;
            for (var i = 0; i < amount; i++)
            {
                response = await client.PostAsync(request, null);
                response.EnsureSuccessStatusCode();
            }

            CurrentView = updateView ? ReadField(await response.Content.ReadAsStringAsync()) : emptyField;
            var (dx, dy) = direction.ToCoordsChange();
            X += dx * amount;
            Y += dy * amount;
            //Console.WriteLine($"After move X:{X} Y:{Y}");
        }

        public async Task Move(Direction direction, bool updateView = true)
        {
            //Console.WriteLine($"X:{X} Y:{Y}");
            var response = await client.PostAsync($"/task/move/{direction}", null);
            response.EnsureSuccessStatusCode();
            CurrentView = updateView ? ReadField(await response.Content.ReadAsStringAsync()) : emptyField;
            var (dx, dy) = direction.ToCoordsChange();
            X += dx;
            Y += dy;
        }

        private static bool[,] ReadField(string fieldString)
        {
            if (string.IsNullOrWhiteSpace(fieldString))
                return emptyField;
            //Console.WriteLine(fieldString);
            //Console.WriteLine();
            var rows = fieldString.Split(new[] {"\r\n"}, StringSplitOptions.None);
            var field = new bool[rows.First().Length, rows.Length];
            for (var x = 0; x < field.GetLength(0); x++)
            for (var y = 0; y < field.GetLength(1); y++)
            {
                field[x, y] = rows[y][x] == '1';
            }

            return field;
        }

        public async Task FinishGameAsync()
        {
            var response = await client.PostAsync("/task/game/finish", null);
            response.EnsureSuccessStatusCode();
            Points = (await response.DeserializeContent<Dictionary<string, int>>())["points"];
        }
    }
}
