using System.ServiceModel;

namespace SharpDevelopRemoteControl.Contracts
{
    [ServiceContract]
    public interface IHostApplicationService
    {
        [OperationContract(IsOneWay = true)]
        void RemoteControlAvailable(string listenUri);

        [OperationContract(IsOneWay = true)]
        void DevelopmentEnvironmentShuttingDown();

        [OperationContract]
        ScriptResult ExecuteScript(string assemblyPath, string className, string methodName);
    }
}