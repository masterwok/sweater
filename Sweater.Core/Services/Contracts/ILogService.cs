using System;

namespace Sweater.Core.Services.Contracts
{
    public interface ILogService<T>
    {
        void LogDebug(string message);

        void LogError(string message, Exception exception = null);
    }
}