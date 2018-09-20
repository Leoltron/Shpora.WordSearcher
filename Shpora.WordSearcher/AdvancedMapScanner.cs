using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Shpora.WordSearcher
{
    public class AdvancedMapScanner : SimpleMapScanner
    {
        public AdvancedMapScanner(WordSearcherGameClient wsGameClient, int mapWidth, int mapHeight)
            : base(wsGameClient, mapWidth, mapHeight)
        {
        }

        [SuppressMessage("ReSharper", "CommentTypo")]
        public override async Task<bool[,]> ScanMapAsync()
        {
            var scanWidth = MapWidth - Math.Max(0, Constants.VisibleFieldWidth - (Constants.LetterSize - 1));

            var mapWriter = new MapWriter(WsGameClient, MapWidth, MapHeight);
            var linesLeftToScan = MapHeight;
            var justMovedDown = false;

            while (linesLeftToScan > 0)
            {
                var lineRelativePosition = 0;
                for (var columnsLeftToScan = scanWidth - Constants.VisibleFieldWidth;
                    columnsLeftToScan > 0;
                    columnsLeftToScan -= Constants.VisibleFieldWidth)
                {
                    var upOffset = SeeAnythingOnTop() ? Constants.LetterSize - 1 : 0;
                    if (justMovedDown || lineRelativePosition < 0)
                        upOffset = 0;
                    var downOffset = SeeAnythingOnBottom() ? Constants.LetterSize - 1 : 0;
                    if (lineRelativePosition > 0)
                        downOffset = 0;

                    if (lineRelativePosition == 0)
                    {
                        if (upOffset != 0)
                        {
                            for (int i = 0; i < upOffset; i++)
                            {
                                await WsGameClient.MoveAsync(Direction.Up);
                                mapWriter.UpdateMap();
                            }

                            lineRelativePosition = -upOffset;
                        }

                        if (downOffset != 0)
                        {
                            if (upOffset != 0)
                                await WsGameClient.MoveAsync(Direction.Down, upOffset, false);
                            for (int i = 0; i < downOffset; i++)
                            {
                                await WsGameClient.MoveAsync(Direction.Down);
                                mapWriter.UpdateMap();
                            }

                            lineRelativePosition = downOffset;
                        }
                    }
                    else
                    {
                        var moveDirection = lineRelativePosition > 0 ? Direction.Up : Direction.Down;
                        for (int i = 0; i < lineRelativePosition; i++)
                        {
                            await WsGameClient.MoveAsync(moveDirection);
                            mapWriter.UpdateMap();
                        }

                        var oppositeOffset = moveDirection == Direction.Up ? upOffset : downOffset;
                        if (oppositeOffset > 0)
                        {
                            for (int i = 0; i < oppositeOffset; i++)
                            {
                                await WsGameClient.MoveAsync(moveDirection);
                                mapWriter.UpdateMap();
                            }

                            lineRelativePosition = oppositeOffset;
                            if (moveDirection == Direction.Up)
                                lineRelativePosition = -lineRelativePosition;
                        }
                        else
                            lineRelativePosition = 0;
                    }

                    if (columnsLeftToScan > Constants.VisibleFieldWidth)
                    {
                        await WsGameClient.MoveAsync(Direction.Right,
                            Math.Min(columnsLeftToScan, Constants.VisibleFieldWidth));
                        mapWriter.UpdateMap();
                    }

                    //TODO: Когда идем, если увидел две пустых линии подряд - идем обратно
                    justMovedDown = false;
                }

                linesLeftToScan -= Constants.LetterSize - 1 + Constants.VisibleFieldHeight;
                if (linesLeftToScan > 0)
                {
                    var amount = Constants.LetterSize - 1 + Math.Min(linesLeftToScan, Constants.VisibleFieldHeight);
                    amount -= lineRelativePosition;
                    await WsGameClient.MoveAsync(Direction.Down, amount);
                    mapWriter.UpdateMap();
                    justMovedDown = true;
                }
            }

            return mapWriter.Map;
        }

        private bool SeeAnythingOnTop()
        {
            for (var i = 0; i < Constants.VisibleFieldWidth; i++)
            {
                if (WsGameClient.CurrentView[i, 0] || WsGameClient.CurrentView[i, 1])
                    return true;
            }

            return false;
        }

        private bool SeeAnythingOnBottom()
        {
            const int bottomY = Constants.VisibleFieldHeight - 1;
            for (var i = 0; i < Constants.VisibleFieldWidth; i++)
            {
                if (WsGameClient.CurrentView[i, bottomY] || WsGameClient.CurrentView[i, bottomY - 1])
                    return true;
            }

            return false;
        }
    }
}
