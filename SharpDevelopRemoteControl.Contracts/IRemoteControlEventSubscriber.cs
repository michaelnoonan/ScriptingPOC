using System.ServiceModel;

namespace SharpDevelopRemoteControl.Contracts
{
    [ServiceContract]
    public interface IRemoteControlEventSubscriber
    {
        [OperationContract(IsOneWay = true)]
        void RemoteControlAvailable(string uri);

        [OperationContract(IsOneWay = true)]
        void ShuttingDown();
    }
}