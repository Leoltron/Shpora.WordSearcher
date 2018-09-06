using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Shpora.WordSearcher
{
    public class WordSearcher
    {
        private readonly HttpClient client;
        public int Points { get; private set; }
        public int Words { get; private set; }
        public int Moves { get; private set; }

        public WordSearcher(HttpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task InitGameAsync(bool test = false)
        {
            var request = "/task/game/start";
            if (test)
                request += "?test=true";

            var response = await client.PostAsync(request, null);
            response.EnsureSuccessStatusCode();
            var secondsTimeout = long.Parse(response.Headers.GetValues("Expires").First());
            var sessionInitDate = DateTime.Parse(string.Join(" ", response.Headers.GetValues("Last-Modified")));
            Points = 0;
        }

        public async Task UpdateStatsAsync()
        {
            const string request = "/task/game/stats";
            var response = await client.GetAsync(request);
            var stats =
                JsonConvert.DeserializeObject<Dictionary<string, int>>(await response.Content.ReadAsStringAsync());
            Words = stats["words"];
            Points = stats["points"];
            Moves = stats["moves"];
        }

        public async Task FinishGameAsync()
        {
            const string request = "/task/game/finish";
            var response = await client.PostAsync(request, null);
            response.EnsureSuccessStatusCode();
            Points =
                JsonConvert.DeserializeObject<Dictionary<string, int>>(await response.Content.ReadAsStringAsync())[
                    "points"];
        }
    }
}