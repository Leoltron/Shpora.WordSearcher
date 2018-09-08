using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

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
            await ws.InitGameAsync(true);
            await ws.UpdateStatsAsync();
            await ws.Move(Direction.Left);
            await ws.Move(Direction.Right);
            var cd = Direction.Right;
            var height = ws.CurrentView.GetLength(1);
            var width = ws.CurrentView.GetLength(0);
            var b = new bool[500, 100];
            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 50; j++)
                {
                    for (var k = 1; k < width; k++)
                    {
                        await ws.Move(cd, false);
                    }

                    await MoveAndUpdate(ws, cd, b);
                }

                for (var j = 1; j < height; j++)
                    await ws.Move(Direction.Down, false);
                await MoveAndUpdate(ws, Direction.Down, b);
                cd = cd == Direction.Right ? Direction.Left : Direction.Right;
            }

            for (var y = 0; y < 100; y++)
            {
                var sb = new StringBuilder(100);
                for (var x = 0; x < 100; x++)
                {
                    sb.Append(b[x, y] ? "#" : "_");
                }

                Console.WriteLine(sb);
            }

            await ws.FinishGameAsync();
        }

        private static async Task MoveAndUpdate(WordSearcher ws, Direction dir, bool[,] map)
        {
            await ws.Move(dir);
            var maxX = ws.CurrentView.GetLength(0);
            var maxY = ws.CurrentView.GetLength(1);
            for (var x = 0; x < maxX; x++)
            for (var y = 0; y < maxY; y++)
            {
                var realX = x + ws.X;
                var realY = y + ws.Y;
                if (realX >= 0 && realY >= 0 && realX < map.GetLength(0) && realY < map.GetLength(1))
                    map[realX, realY] = ws.CurrentView[x, y];
            }
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