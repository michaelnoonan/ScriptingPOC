using System.ServiceModel;

namespace SharpDevelopRemoteControl.Contracts
{
    [ServiceContract]
    public interface IRemoteControlService
    {
        [OperationContract]
        PingResponse Ping(PingRequest request);

        [OperationContract]
        void LogMessage(string message);

        [OperationContract]
        void LoadProject(string projectFilePath);

        [OperationContract]
        void ShutDown();
    }
}