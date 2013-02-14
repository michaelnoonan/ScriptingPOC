namespace MyCoolApp.Domain.Events.DevelopmentEnvironment
{
    public class DevelopmentEnvironmentConnected
    {
        public string ListenUri { get; private set; }

        public DevelopmentEnvironmentConnected(string listenUri)
        {
            ListenUri = listenUri;
        }
    }
}