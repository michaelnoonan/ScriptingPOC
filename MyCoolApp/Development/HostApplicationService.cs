using System.ServiceModel;
using Caliburn.Micro;
using MyCoolApp.Events.DevelopmentEnvironment;
using MyCoolApp.Projects;
using MyCoolApp.Scripting;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Development
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class HostApplicationService : IHostApplicationService
    {
        private readonly IEventAggregator _globalEventAggregator;
        private readonly IProjectManager _projectManager;
        private readonly IScriptingService _scriptingService;

        public HostApplicationService()
        {
            _globalEventAggregator = Program.GlobalEventAggregator;
            _projectManager = ProjectManager.Instance;
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

        public ScriptExecutionResult ExecuteScriptForDebugging(string assemblyName, string className, string methodName)
        {
            return _scriptingService.ExecuteScriptForDebugging(assemblyName, className, methodName);
        }
    }
}