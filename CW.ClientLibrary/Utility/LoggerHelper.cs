using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CW.ClientLibrary.Utility
{
    public class LoggerHelper : Exception
    {
        private ILogger _logger;
        public LoggerHelper(ILogger logger)
        {
            _logger = logger;
            _logger.LogInformation("Logger Helper");
        }

        public LoggerHelper(string message)
            : base(message)
        {
            _logger.LogCritical(message);
        }

        public LoggerHelper(string message, Exception inner)
            : base(message, inner)
        {
            _logger.LogCritical(message, inner);
        }
    }
}
