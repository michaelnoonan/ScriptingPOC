using System.ServiceModel;
using System.Threading.Tasks;

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
        Task<ScriptExecutionResult> ExecuteScriptForDebuggingAsync(string assemblyName, string className, string methodName);

        [OperationContract(IsOneWay = true)]
        void ScriptingProjectLoaded(LoadScriptingProjectResult result);

        [OperationContract(IsOneWay = true)]
        void ScriptingProjectUnloaded();
    }
}