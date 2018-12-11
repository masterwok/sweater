using System;
using Microsoft.Extensions.Logging;
using Sweater.Core.Services.Contracts;

namespace Sweater.Api.Services
{
    /// <summary>
    /// This log service uses Microsoft.Extensions.Logging for logging.
    /// </summary>
    /// <typeparam name="T">The type of class logger is registered against.</typeparam>
    public class LogService<T> : ILogService<T>
    {
        private readonly ILogger<T> _logger;

        public LogService(
            ILogger<T> logger
        ) => _logger = logger;

        public void LogDebug(string message) => _logger.Log(LogLevel.Debug, message);

        public void LogError(
            string message
            , Exception exception = null
        ) => _logger.Log(
            LogLevel.Error
            , exception
            , message
        );
    }
}