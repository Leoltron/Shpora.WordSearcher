using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Shpora.WordSearcher
{
    public class WordSearcherGameClient
    {
        private readonly HttpClient client;

        public bool SessionInProgress { get; private set; }

        public DateTime SessionStartDate { get; private set; }
        public DateTime SessionExpireDate { get; private set; }

        public GameState State { get; private set; }
        public ViewHashRecorder ViewHashRecorder { get; } = new ViewHashRecorder();

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

            var response = await client.PostAsync(request, null, Constants.RequestAttempts);
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                Logger.Log.Warn("Received Conflict response code, probably there is unfinished session, trying again");
                response = await client.PostAsync(request, null, Constants.RequestAttempts);
            }

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
            State = new GameState();
        }

        public async Task UpdateStatsAsync()
        {
            var response = await client.GetAsync("/task/game/stats", Constants.RequestAttempts);
            State.UpdateStats(await response.DeserializeContent<Dictionary<string, int>>());
        }

        public async Task MoveAsync(Direction direction, int amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Value must be positive");
            for (var i = 0; i < amount; i++)
                await MoveAsync(direction);
        }

        public async Task MoveAsync(Direction direction)
        {
            var response = await client.PostAsync($"/task/move/{direction}", null, Constants.RequestAttempts);
            response.EnsureSuccessStatusCode();
            var newView = ReadView(await response.Content.ReadAsStringAsync());
            ViewHashRecorder.AddHash(newView);
            State.UpdateStateFromMove(direction, newView);
        }

        public async Task MoveAsync(int x, int y)
        {
            if (x != 0)
                await MoveAsync(x > 0 ? Direction.Right : Direction.Left, Math.Abs(x));
            if (y != 0)
                await MoveAsync(y > 0 ? Direction.Down : Direction.Up, Math.Abs(y));
        }

        private static bool[,] ReadView(string fieldString)
        {
            if (string.IsNullOrWhiteSpace(fieldString))
                return null;
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
            var response = await client.PostAsync("/task/words/", content, Constants.RequestAttempts);
            var pointsInDict = await response.DeserializeContent<Dictionary<string, int>>();
            var pointsReceived = pointsInDict["points"];
            Logger.Log.Info($"Received {pointsReceived} for " + wordsJson);
            return pointsReceived;
        }

        public async Task FinishGameAsync()
        {
            var response = await client.PostAsync("/task/game/finish", null, Constants.RequestAttempts);
            response.EnsureSuccessStatusCode();
            SessionInProgress = false;
            State.Points = (await response.DeserializeContent<Dictionary<string, int>>())["points"];
        }
    }
}
