using System.ServiceModel;

namespace SharpDevelopRemoteControl.Contracts
{
    [ServiceContract]
    public interface IRemoteControlAnnouncementService
    {
        [OperationContract(IsOneWay = true)]
        void RemoteControlAvailable(string uri);
    }
}