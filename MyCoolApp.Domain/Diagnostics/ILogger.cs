using System;

namespace MyCoolApp.Domain.Diagnostics
{
    public interface ILogger
    {
        void Info(string format, params object[] args);
        void Error(Exception ex, string format, params object[] args);
        void Error(string format, params object[] args);
    }
}