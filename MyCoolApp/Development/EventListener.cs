using MyCoolApp.Events.DevelopmentEnvironment;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Development
{
    public class EventListener : IDevelopmentEnvironmentEventListener
    {
        public void RemoteControlAvailable(string listenUri)
        {
            Program.GlobalEventAggregator.Publish(new RemoteControlStarted(listenUri));
        }

        public void ShuttingDown()
        {
            Program.GlobalEventAggregator.Publish(new RemoteControlShutDown());
        }
    }
}