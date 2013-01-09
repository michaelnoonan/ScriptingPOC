using System.ServiceModel;

namespace SharpDevelopRemoteControl.Contracts
{
    [ServiceContract]
    public interface IRemoteControl
    {
        [OperationContract]
        void LogMessage(string message);

        [OperationContract]
        void LoadProject(string projectFilePath);

        [OperationContract]
        void ShutDown();
    }
}