namespace Shpora.WordSearcher
{
    public class MapWriter
    {
        private readonly WordSearcherGameClient wsGameClient;
        private readonly bool[,] map;

        public bool[,] Map => (bool[,]) map.Clone();

        public MapWriter(WordSearcherGameClient wsGameClient, int mapWidth, int mapHeight)
        {
            this.wsGameClient = wsGameClient;
            map = new bool[mapWidth, mapHeight];
        }

        public void UpdateMap()
        {
            CopyMapFromWsClient(map);
        }

        private void CopyMapFromWsClient(bool[,] to)
        {
            CopyMap(wsGameClient.State.CurrentView, to, wsGameClient.State.X, wsGameClient.State.Y);
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
    }
}
