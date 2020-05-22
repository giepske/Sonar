namespace Sonar.Logging
{
    public interface ILogger
    {
        public void Log(LogType logType, string message);
    }
}
