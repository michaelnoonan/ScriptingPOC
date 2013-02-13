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
        ScriptExecutionResult ExecuteScript(string assemblyPath, string className, string methodName);

        [OperationContract]
        ScriptLoadResult LoadScript(string assemblyPath, string className, string methodName);
    }
}