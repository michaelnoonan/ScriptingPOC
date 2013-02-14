namespace MyCoolApp.Domain.Events.Diagnostics
{
    public class LogErrorEvent
    {
        public LogErrorEvent(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}