using System;

namespace Shpora.WordSearcher
{
    public static class Logger
    {
        public enum Level
        {
            Debug,
            Info,
            Warn,
            Error
        }

        private static void WriteColoredLine(string message, ConsoleColor textColor)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = textColor;
            Console.WriteLine(message);
            Console.ForegroundColor = oldColor;
        }

        private static string GetLogDateTimeString()
        {
            return DateTime.UtcNow.ToLongTimeString();
        }

        private static void Log(Level level, string message, Exception exception = null)
        {
            if (string.IsNullOrWhiteSpace(message) && exception == null)
                return;

            if (string.IsNullOrWhiteSpace(message))
                message = "An exception has occured:";

            var logLine = $"[{level.GetLevelTag()}] [{GetLogDateTimeString()}] {message}";
            if (exception != null)
                logLine += Environment.NewLine + exception;

            WriteColoredLine(logLine, level.GetConsoleColor());
        }

        public static void Debug(Exception exception)
        {
            Debug(null, exception);
        }

        public static void Debug(string message, Exception exception = null)
        {
            Log(Level.Debug, message, exception);
        }

        public static void Info(Exception exception)
        {
            Info(null, exception);
        }

        public static void Info(string message, Exception exception = null)
        {
            Log(Level.Info, message, exception);
        }

        public static void Warn(Exception exception)
        {
            Warn(null, exception);
        }

        public static void Warn(string message, Exception exception = null)
        {
            Log(Level.Warn, message, exception);
        }

        public static void Error(Exception exception)
        {
            Error(null, exception);
        }

        public static void Error(string message, Exception exception = null)
        {
            Log(Level.Error, message, exception);
        }
    }
}
