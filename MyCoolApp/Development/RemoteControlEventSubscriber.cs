using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Development
{
    public class RemoteControlEventSubscriber : IRemoteControlEventSubscriber
    {
        public void RemoteControlAvailable(string uri)
        {
            RemoteControlManager.Instance.RemoteControlAvailableAt(uri);
        }

        public void ShuttingDown()
        {
            RemoteControlManager.Instance.RemoteControlShutDown();
        }
    }
}