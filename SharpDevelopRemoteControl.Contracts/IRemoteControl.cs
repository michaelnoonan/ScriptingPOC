using System.ServiceModel;

namespace SharpDevelopRemoteControl.Contracts
{
    [ServiceContract]
    public interface IRemoteControl
    {
        [OperationContract]
        void LogMessage(string message);

        [OperationContract]
        void LoadScriptingProject(string projectFilePath);

        [OperationContract]
        void ShutDown();
    }
}