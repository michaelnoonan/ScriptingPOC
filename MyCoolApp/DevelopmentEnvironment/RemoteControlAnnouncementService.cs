using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.DevelopmentEnvironment
{
    public class RemoteControlAnnouncementService : IRemoteControlAnnouncementService
    {
        public void RemoteControlAvailable(string uri)
        {
            DevelopmentEnvironmentHub.Instance.SetRemoteControlUri(uri);
        }
    }
}