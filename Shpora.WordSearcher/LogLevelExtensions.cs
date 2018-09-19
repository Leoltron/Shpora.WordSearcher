using System;
using static Shpora.WordSearcher.Logger;

namespace Shpora.WordSearcher
{
    public static class LogLevelExtensions
    {
        public static string GetLevelTag(this Level level)
        {
            switch (level)
            {
                case Level.Debug:
                    return "DEBUG";
                case Level.Info:
                    return "INFO";
                case Level.Warn:
                    return "WARN";
                case Level.Error:
                    return "ERROR";
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public static ConsoleColor GetConsoleColor(this Level level)
        {
            switch (level)
            {
                case Level.Debug:
                    return ConsoleColor.Gray;
                case Level.Info:
                    return ConsoleColor.White;
                case Level.Warn:
                    return ConsoleColor.Yellow;
                case Level.Error:
                    return ConsoleColor.Red;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }
    }
}
