using System;
using System.Threading.Tasks;

namespace Shpora.WordSearcher
{
    public class SimpleMapScanner
    {
        protected readonly WordSearcherGameClient WsGameClient;
        protected readonly int MapWidth;
        protected readonly int MapHeight;

        public SimpleMapScanner(WordSearcherGameClient wsGameClient, int mapWidth, int mapHeight)
        {
            WsGameClient = wsGameClient;
            MapWidth = mapWidth;
            MapHeight = mapHeight;
        }

        public virtual async Task<bool[,]> ScanMapAsync()
        {
            var mapWriter = new MapWriter(WsGameClient, MapWidth, MapHeight);

            var linesRemain = MapHeight;
            mapWriter.UpdateMap();
            Logger.Debug("Scan progress: 0%");
            while (true)
            {
                for (var i = 0; i < MapWidth - Constants.VisibleFieldWidth; i++)
                {
                    await WsGameClient.MoveAsync(Direction.Right);
                    if (i % Constants.VisibleFieldWidth == 0)
                        mapWriter.UpdateMap();
                }

                mapWriter.UpdateMap();
                linesRemain -= Constants.VisibleFieldHeight;
                if (linesRemain <= 0) break;
                await WsGameClient.MoveAsync(Direction.Down, Math.Min(linesRemain, Constants.VisibleFieldHeight));
                mapWriter.UpdateMap();
                Logger.Debug($"Scan progress: {(int) (100 - (float) linesRemain / MapHeight * 100)}%");
            }

            Logger.Debug("Scan progress: 100%");

            return mapWriter.Map;
        }
    }
}
