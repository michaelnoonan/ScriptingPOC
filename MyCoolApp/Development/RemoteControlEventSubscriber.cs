using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Development
{
    public class RemoteControlEventSubscriber : IRemoteControlEventSubscriber
    {
        public void RemoteControlAvailable(string uri)
        {
            DevelopmentEnvironmentManager.Instance.RemoteControlAvailableAt(uri);
        }

        public void ShuttingDown()
        {
            DevelopmentEnvironmentManager.Instance.RemoteControlShutDown();
        }
    }
}