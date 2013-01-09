namespace MyCoolApp.Events.DevelopmentEnvironment
{
    public class RemoteControlStarted
    {
        public string ListenUri { get; private set; }

        public RemoteControlStarted(string listenUri)
        {
            ListenUri = listenUri;
        }
    }
}