using System.Threading.Tasks;

namespace Shpora.WordSearcher
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var (server, token) = ParseArgs(args);
            Logger.Info($"Playing on server \"{server}\" with token \"{token}\"");
            var wsClient = new WordSearcherGameClient(server, token);
            var ws = new WordSearcher(wsClient);
            await ws.PlaySessionAsync();
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
