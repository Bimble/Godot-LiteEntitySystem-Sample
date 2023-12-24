using Microsoft.Extensions.Logging;

namespace Sample.Server
{
    internal class ConsoleLogger : LiteEntitySystem.ILogger
    {
        private ILogger _logger;

        public ConsoleLogger() 
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = factory.CreateLogger("Program");
        }

        public void Log(string log)
        {
            _logger.LogInformation(log);
        }

        public void LogError(string log)
        {
            _logger.LogError(log);
        }

        public void LogWarning(string log)
        {
            _logger.LogWarning(log);
        }
    }
}
