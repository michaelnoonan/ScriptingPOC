using System;
using Caliburn.Micro;
using MyCoolApp.Domain.Events.Diagnostics;

namespace MyCoolApp.Domain.Diagnostics
{
    public class Logger : ILogger
    {
        public static Logger Instance = new Logger(GlobalEventAggregator.Instance);

        private readonly IEventAggregator _globalEventAggregator;

        public Logger(IEventAggregator globalEventAggregator)
        {
            _globalEventAggregator = globalEventAggregator;
        }

        public void Info(string format, params object[] args)
        {
            _globalEventAggregator.Publish(new LogInfoEvent(string.Format(format, args)));
        }

        public void Error(Exception ex, string format, params object[] args)
        {
            _globalEventAggregator.Publish(new LogErrorEvent(string.Format(format, args) + Environment.NewLine + ex));
        }

        public void Error(string format, params object[] args)
        {
            _globalEventAggregator.Publish(new LogErrorEvent(string.Format(format, args)));
        }
    }
}