using System;
using System.Threading.Tasks;
using static Shpora.WordSearcher.Direction;

namespace Shpora.WordSearcher
{
    public class SimpleMapScanner
    {
        protected readonly WordSearcherGameClient wsGameClient;
        protected readonly int mapWidth;
        protected readonly int mapHeight;

        public SimpleMapScanner(WordSearcherGameClient wsGameClient, int mapWidth, int mapHeight)
        {
            this.wsGameClient = wsGameClient;
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
        }

        public virtual async Task<bool[,]> ScanMapAsync()
        {
            var map = new bool[mapWidth, mapHeight];
            void UpdateMap() => CopyMapFromWsClient(map);

            var linesRemain = mapHeight;
            UpdateMap();
            Logger.Debug("Scan progress: 0%");
            while (true)
            {
                for (var i = 0; i < mapWidth - Constants.VisibleFieldWidth; i++)
                {
                    await wsGameClient.MoveAsync(Right);
                    if (i % Constants.VisibleFieldWidth == 0)
                        UpdateMap();
                }

                UpdateMap();
                linesRemain -= Constants.VisibleFieldHeight;
                if (linesRemain <= 0) break;
                await wsGameClient.MoveAsync(Down, Math.Min(linesRemain, Constants.VisibleFieldHeight));
                UpdateMap();
                Logger.Debug($"Scan progress: {(int) (100 - (float) linesRemain / mapHeight * 100)}%");
            }

            Logger.Debug("Scan progress: 100%");

            return map;
        }

        protected void CopyMapFromWsClient(bool[,] to)
        {
            CopyMap(wsGameClient.CurrentView, to, wsGameClient.X, wsGameClient.Y);
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
