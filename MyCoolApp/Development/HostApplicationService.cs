using Caliburn.Micro;
using MyCoolApp.Events.DevelopmentEnvironment;
using MyCoolApp.Projects;
using MyCoolApp.Scripting;
using SharpDevelopRemoteControl.Contracts;

namespace MyCoolApp.Development
{
    public class HostApplicationService : IHostApplicationService
    {
        private readonly EventAggregator _globalEventAggregator;

        public HostApplicationService()
        {
            _globalEventAggregator = Program.GlobalEventAggregator;
        }

        public void RemoteControlAvailable(string listenUri)
        {
            _globalEventAggregator.Publish(new DevelopmentEnvironmentConnected(listenUri));
        }

        public void DevelopmentEnvironmentShuttingDown()
        {
            _globalEventAggregator.Publish(new DevelopmentEnvironmentDisconnected());
        }

        public ScriptResult ExecuteFileAsScript(string scriptFilePath)
        {
            var executor = new ScriptExecutor(ProjectManager.Instance.Project);
            return executor.ExecuteScriptFile(scriptFilePath);
        }
    }
}