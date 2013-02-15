using System.ServiceModel;
using System.Threading.Tasks;
using Caliburn.Micro;
using MyCoolApp.Domain.Events.DevelopmentEnvironment;
using MyCoolApp.Domain.Scripting;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Domain.Development
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class HostApplicationService : IHostApplicationService
    {
        private readonly IEventAggregator _globalEventAggregator;
        private readonly IScriptingService _scriptingService;

        public HostApplicationService()
        {
            _globalEventAggregator = GlobalEventAggregator.Instance;
            _scriptingService = ScriptingService.Instance;
        }

        public void RemoteControlAvailable(string listenUri)
        {
            _globalEventAggregator.Publish(new DevelopmentEnvironmentConnected(listenUri));
        }

        public void DevelopmentEnvironmentShuttingDown()
        {
            _globalEventAggregator.Publish(new DevelopmentEnvironmentDisconnected());
        }

        public Task<ScriptExecutionResult> ExecuteScriptForDebuggingAsync(string assemblyName, string className, string methodName)
        {
            return _scriptingService.ExecuteScriptForDebuggingAsync(assemblyName, className, methodName);
        }

        public void ScriptingProjectLoaded(LoadScriptingProjectResult result)
        {
            _globalEventAggregator.Publish(new ScriptingProjectLoadedInDevelopmentEnvironment(result));
        }

        public void ScriptingProjectUnloaded()
        {
            _globalEventAggregator.Publish(new ScriptingProjectUnloadedInDevelopmentEnvironment());
        }
    }
}