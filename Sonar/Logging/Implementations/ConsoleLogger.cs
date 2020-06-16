using System;

namespace Sonar.Logging.Implementations
{
    public class ConsoleLogger : ILogger
    {
        private readonly object _lockObject = new object();

        public void Log(LogType logType, string message)
        {
            lock (_lockObject)
            {
                Console.Write("[");

                if (logType == LogType.Info)
                {
                    SetConsoleColor(ConsoleColor.Blue);
                }
                else if (logType == LogType.Warning)
                {
                    SetConsoleColor(ConsoleColor.DarkYellow);
                }
                else if (logType == LogType.Error)
                {
                    SetConsoleColor(ConsoleColor.Red);
                }

                Console.Write($"{logType}");

                SetConsoleColor(ConsoleColor.White);

                Console.WriteLine($"] {message}");
            }
        }

        private void SetConsoleColor(ConsoleColor textColor)
        {
            Console.ForegroundColor = textColor;
        }
    }
}
