using System;
using MyCoolApp.Events.Diagnostics;

namespace MyCoolApp
{
    public class Logger
    {
        public static Logger Instance = new Logger();

        public void Info(string format, params object[] args)
        {
            Program.GlobalEventAggregator.Publish(new LogInfoEvent(string.Format(format, args)));
        }

        public void Error(Exception ex, string format, params object[] args)
        {
            Program.GlobalEventAggregator.Publish(new LogErrorEvent(string.Format(format, args) + Environment.NewLine + ex));
        }
    }
}