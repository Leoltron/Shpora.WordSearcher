using System;
using System.Threading.Tasks;

namespace Shpora.WordSearcher
{
    public class SimpleMapScanner
    {
        private readonly WordSearcherGameClient wsGameClient;
        private readonly int mapWidth;
        private readonly int mapHeight;

        public SimpleMapScanner(WordSearcherGameClient wsGameClient, int mapWidth, int mapHeight)
        {
            this.wsGameClient = wsGameClient;
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
        }

        public async Task<bool[,]> ScanMapAsync()
        {
            var mapWriter = new MapWriter(wsGameClient, mapWidth, mapHeight);

            var linesRemain = mapHeight;
            mapWriter.UpdateMap();
            Logger.Log.Debug("Scan progress: 0%");
            while (true)
            {
                for (var i = 0; i < mapWidth - Constants.VisibleFieldWidth; i++)
                {
                    await wsGameClient.MoveAsync(Direction.Right);
                    if (i % Constants.VisibleFieldWidth == 0)
                        mapWriter.UpdateMap();
                }

                mapWriter.UpdateMap();
                linesRemain -= Constants.VisibleFieldHeight;
                if (linesRemain <= 0) break;
                await wsGameClient.MoveAsync(Direction.Down, Math.Min(linesRemain, Constants.VisibleFieldHeight));
                mapWriter.UpdateMap();
                Logger.Log.Debug($"Scan progress: {(int) (100 - (float) linesRemain / mapHeight * 100)}%");
            }

            Logger.Log.Debug("Scan progress: 100%");

            return mapWriter.Map;
        }
    }
}
