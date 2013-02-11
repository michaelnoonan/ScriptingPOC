namespace MyCoolApp.Events.Diagnostics
{
    public class LogInfoEvent
    {
        public LogInfoEvent(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}