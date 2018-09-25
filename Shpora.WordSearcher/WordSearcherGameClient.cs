using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Shpora.WordSearcher
{
    public class WordSearcherGameClient
    {
        private static readonly bool[,] EmptyField = new bool[0, 0];
        private readonly HttpClient client;

        public bool SessionInProgress { get; private set; }

        public int Points { get; private set; }
        public int Words { get; private set; }
        public int Moves { get; private set; }

        public bool[,] CurrentView { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }

        public DateTime SessionStartDate { get; private set; }
        public DateTime SessionExpireDate { get; private set; }

        public bool SeesAnything => CurrentView?.Any(b => b) ?? false;

        public WordSearcherGameClient(string serverUrl, string token)
        {
            if (string.IsNullOrWhiteSpace(serverUrl))
                throw new ArgumentException("Server url cannot be empty", nameof(serverUrl));

            client = new HttpClient {BaseAddress = new Uri(serverUrl)};
            if (!string.IsNullOrWhiteSpace(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
        }

        public async Task InitGameAsync(bool test = false)
        {
            var request = "/task/game/start";
            if (test)
                request += "?test=true";

            var response = await client.PostAsync(request, null);
            response.EnsureSuccessStatusCode();
            SessionInProgress = true;

            var contentHeaders = response.Content.Headers;
            var sessionTimeoutSeconds = long.Parse(contentHeaders.GetValues("Expires").First());
            SessionStartDate = DateTime.Parse(contentHeaders.GetValues("Last-Modified").First());
            SessionExpireDate = SessionStartDate.AddSeconds(sessionTimeoutSeconds);

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

        public async Task MoveAsync(Direction direction, int amount, bool updateView = true)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Value must be positive");
            var request = $"/task/move/{direction}";
            HttpResponseMessage response = null;
            for (var i = 0; i < amount; i++)
            {
                response = await client.PostAsync(request, null);
                response.EnsureSuccessStatusCode();
            }

            CurrentView = updateView ? ReadField(await response.Content.ReadAsStringAsync()) : EmptyField;
            var (dx, dy) = direction.ToCoordsChange();
            X += dx * amount;
            Y += dy * amount;
        }

        public async Task MoveAsync(Direction direction, bool updateView = true)
        {
            var response = await client.PostAsync($"/task/move/{direction}", null);
            response.EnsureSuccessStatusCode();
            CurrentView = updateView ? ReadField(await response.Content.ReadAsStringAsync()) : EmptyField;
            var (dx, dy) = direction.ToCoordsChange();
            X += dx;
            Y += dy;
        }

        private static bool[,] ReadField(string fieldString)
        {
            if (string.IsNullOrWhiteSpace(fieldString))
                return EmptyField;
            var rows = fieldString.Split(new[] {"\r\n"}, StringSplitOptions.None);
            var field = new bool[rows.First().Length, rows.Length];
            for (var x = 0; x < field.GetLength(0); x++)
            for (var y = 0; y < field.GetLength(1); y++)
            {
                field[x, y] = rows[y][x] == '1';
            }

            return field;
        }

        public async Task<int> SubmitWordsAsync(params string[] words)
        {
            var wordsJson = JsonConvert.SerializeObject(words);
            var content = new StringContent(wordsJson, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/task/words/", content);
            var pointsInDict = await response.DeserializeContent<Dictionary<string, int>>();
            var pointsReceived = pointsInDict["points"];
            Logger.Log.Info($"Received {pointsReceived} for " + wordsJson);
            return pointsReceived;
        }

        public async Task FinishGameAsync()
        {
            var response = await client.PostAsync("/task/game/finish", null);
            response.EnsureSuccessStatusCode();
            SessionInProgress = false;
            Points = (await response.DeserializeContent<Dictionary<string, int>>())["points"];
        }

        public void LogStats()
        {
            Logger.Log.Info(string.Join(Environment.NewLine + "\t",
                "Session finished. Results:",
                "Points: " + Points,
                "Moves: " + Moves,
                "Points from words: " + (Points + Moves),
                "Words submitted: " + Words
            ));
        }
    }
}
