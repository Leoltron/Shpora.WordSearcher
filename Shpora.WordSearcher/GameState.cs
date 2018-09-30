using System;
using System.Collections.Generic;
using Shpora.WordSearcher.Extensions;

namespace Shpora.WordSearcher
{
    public class GameState
    {
        private static readonly bool[,] EmptyField = new bool[0, 0];

        public int Points { get; set; }
        public int Words { get; set; }
        public int Moves { get; set; }

        public bool[,] CurrentView { get; private set; }
        public bool SeesAnything => CurrentView?.Any(b => b) ?? false;

        public int X { get; private set; }
        public int Y { get; private set; }


        public void UpdateStats(Dictionary<string, int> stats)
        {
            Words = stats["words"];
            Points = stats["points"];
            Moves = stats["moves"];
        }

        public void UpdateStateFromMove(Direction direction, bool[,] newView = null, int stepsMade = 1)
        {
            CurrentView = newView ?? EmptyField;
            var (dx, dy) = direction.ToCoordsChange();
            X += dx * stepsMade;
            Y += dy * stepsMade;
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
