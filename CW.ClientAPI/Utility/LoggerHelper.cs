using System;
using Microsoft.Extensions.Logging;

namespace CW.ClientAPI.Utility
{
    public class LoggerHelper  
    {
        private readonly ILogger _logger;
        public LoggerHelper(ILogger logger)
        {
            _logger = logger;
            _logger.LogInformation("Logger Helper");
        }

        public LoggerHelper(string message)
            
        {
            _logger.LogCritical(message);
        }

        public LoggerHelper(string message, Exception inner)
        {
            _logger.LogCritical(message, inner);
        }
    }
}
