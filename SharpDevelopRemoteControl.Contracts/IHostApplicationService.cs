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
        ScriptExecutionResult ExecuteScriptForDebugging(string assemblyName, string className, string methodName);

        [OperationContract(IsOneWay = true)]
        void ScriptingProjectUnloaded();
    }
}