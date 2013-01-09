using System.ServiceModel;

namespace SharpDevelopRemoteControl.Contracts
{
    [ServiceContract]
    public interface IDevelopmentEnvironmentEventListener
    {
        [OperationContract(IsOneWay = true)]
        void RemoteControlAvailable(string listenUri);

        [OperationContract(IsOneWay = true)]
        void ShuttingDown();
    }
}